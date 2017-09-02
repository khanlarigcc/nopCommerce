using System.Collections.Generic;
using Nop.Core.Domain.Stores;

namespace Nop.Services.Stores
{
    /// <summary>
    /// Store service interface
    /// </summary>
    public partial interface IStoreService
    {
        /// <summary>
        /// Deletes a store
        /// </summary>
        /// <param name="store">Store</param>
        void DeleteStore(Store store);

        /// <summary>
        /// Gets all stores
        /// </summary>
        /// <returns>Stores</returns>
        IList<Store> GetAllStores();
        /// <summary>
        /// Gets all stores (cached store entities for performance optimization)
        /// </summary>
        /// <returns>Stores</returns>
        IList<StoreForCaching> GetAllCachedStores();

        /// <summary>
        /// Gets a store 
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Store</returns>
        Store GetStoreById(int storeId);
        /// <summary>
        /// Gets a store (cached store entity for performance optimization)
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Store</returns>
        StoreForCaching GetCachedStoreById(int storeId);

        /// <summary>
        /// Inserts a store
        /// </summary>
        /// <param name="store">Store</param>
        void InsertStore(Store store);

        /// <summary>
        /// Updates the store
        /// </summary>
        /// <param name="store">Store</param>
        void UpdateStore(Store store);
    }
}