using BookmarkDataProvider.CRUDEvents;
using BookmarkDtos;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BookmarkDataProvider
{
    /// <summary>
    /// Provides a base of C.R.U.D. operations for a table that has a single id and no foreign keys.
    /// Create, Read, Update, Delete
    /// </summary>
    public abstract class DataProviderCRUDBase<T> where T : IdBasedObject
    {
        private LiteDatabase db;
        protected string CollectionName { get; set; }
        private LiteCollection<T> collection;

        public delegate void DataProviderRecordCreatedEvent(DataProviderRecordCreatedEventArgs createdRecordEventArgs);
        public delegate void DataProviderRecordUpdatedEvent(DataProviderRecordUpdatedEventArgs updatedRecordEventArgs);
        public delegate void DataProviderRecordDeletedEvent(DataProviderRecordDeletedEventArgs deletedRecordEventArgs);

        /// <summary>
        /// Raised after a record is created and inserted into the database.
        /// The object passed in the event args will have the newly inserted Id.
        /// </summary>
        public DataProviderRecordCreatedEvent OnRecordCreated;

        /// <summary>
        /// Raised after a record has been updated in the database.
        /// The objects in the event args will represent both the before and after state of the affected Dto.
        /// </summary>
        public DataProviderRecordUpdatedEvent OnRecordUpdated;

        /// <summary>
        /// Raised just before the delete operation is performed.
        /// The object passed in the event args will the the record that will be deleted.
        /// </summary>
        public DataProviderRecordDeletedEvent OnBeforeRecordDeleted;

        /// <summary>
        /// Raised after the delete operation is performed.
        /// The object passed in the event args will the the record that was deleted.
        /// </summary>
        public DataProviderRecordDeletedEvent OnAfterRecordDeleted;

        public DataProviderCRUDBase(LiteDatabase liteDatabase, string collectionName)
        {
            this.db = liteDatabase;
            this.CollectionName = collectionName;
            collection = db.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// Returns all elements of the collection.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAll()
        {
            var results = collection.FindAll();
            return results;
        }

        /// <summary>
        /// Gets all results added to a dictionary where the key is the record id
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<int, T> GetAll_AsLookup()
        {
            return GetAll()
                .ToDictionary(v => v.Id, v => v);
        }

        protected virtual IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            var results = collection.Find(expression);
            return results;
        }

        protected virtual int Count(Expression<Func<T, bool>> expression)
        {
            var results = collection.Count(expression);
            return results;
        }

        public virtual T Get(int id)
        {
            return collection.Find(s => s.Id == id).FirstOrDefault();
        }

        public virtual CUDResult Insert(T insertingObject)
        {
            var opResult = new CUDResult();
            if (CUDDepthTracking.ExceedsMaxOperationDepth(opResult))
            {
                return opResult;
            }

            if (insertingObject.Id != 0)
            {
                opResult.Errors.Add($"Cannot insert a new item into {CollectionName}. New items must have their id set to 0 before insert.");
                return opResult;
            }

            collection.Insert(insertingObject);

            try
            {
                CUDDepthTracking.OperationDepth++;
                OnRecordCreated?.Invoke(
                    new DataProviderRecordCreatedEventArgs()
                    {
                        CreatedDto = insertingObject
                    });
            }
            catch (Exception ex)
            {
                opResult.Errors.Add(ex.ToString());
            }
            finally
            {
                CUDDepthTracking.OperationDepth--;
            }
            return opResult;
        }

        public virtual CUDResult Update(T updatingObject)
        {
            var opResult = new CUDResult();
            if (CUDDepthTracking.ExceedsMaxOperationDepth(opResult))
            {
                return opResult;
            }

            var updateEvent = new DataProviderRecordUpdatedEventArgs()
            {
                DtoBefore = Get(updatingObject.Id),
                DtoAfter = updatingObject,
            };

            if (collection.Update(updatingObject) == false)
            {
                opResult.Errors.Add($"An item in {CollectionName} with id {updatingObject.Id} was not found to update.");
                return opResult;
            }

            try
            {
                CUDDepthTracking.OperationDepth++;
                OnRecordUpdated?.Invoke(updateEvent);
            }
            catch (Exception ex)
            {
                opResult.Errors.Add(ex.ToString());
            }
            finally
            {
                CUDDepthTracking.OperationDepth--;
            }

            return opResult;
        }

        public virtual CUDResult Delete(int deletingId)
        {
            var opResult = new CUDResult();
            if (CUDDepthTracking.ExceedsMaxOperationDepth(opResult))
            {
                return opResult;
            }

            var dtoBefore = Get(deletingId);

            try
            {
                CUDDepthTracking.OperationDepth++;
                OnBeforeRecordDeleted?.Invoke(
                    new DataProviderRecordDeletedEventArgs()
                    {
                        DtoBefore = dtoBefore,
                    });
            }
            catch (Exception ex)
            {
                opResult.Errors.Add(ex.ToString());
            }
            finally
            {
                CUDDepthTracking.OperationDepth--;
            }

            if (!collection.Delete(deletingId))
            {
                opResult.Errors.Add($"{CollectionName} with id {deletingId} was not found to delete.");
            }

            try
            {
                CUDDepthTracking.OperationDepth++;
                OnAfterRecordDeleted?.Invoke(
                    new DataProviderRecordDeletedEventArgs()
                    {
                        DtoBefore = dtoBefore,
                    });
            }
            catch (Exception ex)
            {
                opResult.Errors.Add(ex.ToString());
            }
            finally
            {
                CUDDepthTracking.OperationDepth--;
            }

            return opResult;
        }
    }
}
