using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Stores;
using Nop.Services.Events;

namespace Nop.Services.Stores
{
    /// <summary>
    /// Store service
    /// </summary>
    public partial class StoreService : IStoreService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string STORES_ALL_KEY = "Nop.stores.all";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        private const string STORES_BY_ID_KEY = "Nop.stores.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string STORES_PATTERN_KEY = "Nop.stores.";

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string CACHEDSTORES_ALL_KEY = "Nop.cachedstores.all";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        private const string CACHEDSTORES_BY_ID_KEY = "Nop.cachedstores.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CACHEDSTORES_PATTERN_KEY = "Nop.cachedstores.";

        #endregion

        #region Fields

        private readonly IRepository<Store> _storeRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="staticCacheManager">Static cache manager</param>
        /// <param name="storeRepository">Store repository</param>
        /// <param name="eventPublisher">Event published</param>
        public StoreService(ICacheManager cacheManager,
            IStaticCacheManager staticCacheManager,
            IRepository<Store> storeRepository,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._staticCacheManager = staticCacheManager;
            this._storeRepository = storeRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a store
        /// </summary>
        /// <param name="store">Store</param>
        public virtual void DeleteStore(Store store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            var allStores = GetAllStores();
            if (allStores.Count == 1)
                throw new Exception("You cannot delete the only configured store");

            _storeRepository.Delete(store);

            _cacheManager.RemoveByPattern(STORES_PATTERN_KEY);
            _staticCacheManager.RemoveByPattern(CACHEDSTORES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(store);
        }

        /// <summary>
        /// Gets all stores
        /// </summary>
        /// <returns>Stores</returns>
        public virtual IList<Store> GetAllStores()
        {
            return _cacheManager.Get(STORES_ALL_KEY, () =>
            {
                var query = from s in _storeRepository.Table
                            orderby s.DisplayOrder, s.Id
                            select s;
                var stores = query.ToList();
                return stores;
            });
        }
        /// <summary>
        /// Gets all stores (cached store entities for performance optimization)
        /// </summary>
        /// <returns>Stores</returns>
        public virtual IList<StoreForCaching> GetAllCachedStores()
        {
            return _staticCacheManager.Get(CACHEDSTORES_ALL_KEY, () =>
            {
                return GetAllStores().Select(s => new StoreForCaching(s)).ToList();
            });
        }

        /// <summary>
        /// Gets a store 
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Store</returns>
        public virtual Store GetStoreById(int storeId)
        {
            if (storeId == 0)
                return null;
            
            string key = string.Format(STORES_BY_ID_KEY, storeId);
            return _cacheManager.Get(key, () => _storeRepository.GetById(storeId));
        }
        /// <summary>
        /// Gets a store (cached store entity for performance optimization)
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Store</returns>
        public virtual StoreForCaching GetCachedStoreById(int storeId)
        {
            string key = string.Format(STORES_BY_ID_KEY, storeId);
            return _staticCacheManager.Get(key, () =>
            {
                var store = GetStoreById(storeId);
                if (store == null)
                    return null;

                return new StoreForCaching(store);
            });
        }

        /// <summary>
        /// Inserts a store
        /// </summary>
        /// <param name="store">Store</param>
        public virtual void InsertStore(Store store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            _storeRepository.Insert(store);

            _cacheManager.RemoveByPattern(STORES_PATTERN_KEY);
            _staticCacheManager.RemoveByPattern(CACHEDSTORES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(store);
        }

        /// <summary>
        /// Updates the store
        /// </summary>
        /// <param name="store">Store</param>
        public virtual void UpdateStore(Store store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            _storeRepository.Update(store);

            _cacheManager.RemoveByPattern(STORES_PATTERN_KEY);
            _staticCacheManager.RemoveByPattern(CACHEDSTORES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(store);
        }

        #endregion
    }
}