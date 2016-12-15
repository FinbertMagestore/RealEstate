using RealEstateWebUI.Areas.admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using RealEstateWebUI.Areas.admin.UtilzGeneral;

namespace RealEstateWebUI.Areas.admin.Services
{
    public class CollectionProductService
    {
        private IDbConnection connect = new SqlConnection(Common.ConnectString);
        private ProductService productService = new ProductService();
        private CollectionService collectionService = new CollectionService();

        /// <summary>
        /// check exist collection product contains collectionID and productID
        /// </summary>
        /// <param name="collectionID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public bool CheckExistCollectionProduct(int collectionID, int productID)
        {
            try
            {
                string query = string.Format("select * from CollectionProduct where CollectionID = {0} and ProductID = {1}", collectionID, productID);
                List<CollectionProduct> listCollectionProduct = connect.Query<CollectionProduct>(query).ToList<CollectionProduct>();
                if (listCollectionProduct != null && listCollectionProduct.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
            }
            return false;
        }

        /// <summary>
        /// get list collection product by collectionID
        /// </summary>
        /// <param name="collectionID"></param>
        /// <returns></returns>
        public List<CollectionProduct> GetByCollectionID(int collectionID)
        {
            try
            {
                string query = "select * from CollectionProduct where CollectionID = " + collectionID;
                List<CollectionProduct> collectionProducts = connect.Query<CollectionProduct>(query).ToList<CollectionProduct>();
                foreach (var item in collectionProducts)
                {
                    item.Collection = collectionService.GetByPrimaryKey(item.CollectionID);
                    item.Product = productService.GetByPrimaryKey(item.ProductID);
                }
                return collectionProducts;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }
        /// <summary>
        /// get list collection product by productID
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public List<CollectionProduct> GetByProductID(int productID)
        {
            try
            {
                string query = "select * from CollectionProduct where ProductID = " + productID;
                List<CollectionProduct> CollectionProducts = connect.Query<CollectionProduct>(query).ToList<CollectionProduct>();
                foreach (var item in CollectionProducts)
                {
                    item.Collection = collectionService.GetByPrimaryKey(item.CollectionID);
                    item.Product = productService.GetByPrimaryKey(item.ProductID);
                }
                return CollectionProducts;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        /// <summary>
        /// get list product by collectionID
        /// </summary>
        /// <param name="collectionID"></param>
        /// <returns></returns>
        public List<Product> GetProductByCollectionID(int collectionID)
        {
            try
            {
                string query = "select ProductID from CollectionProduct where CollectionID = " + collectionID;
                List<int> listProductID = connect.Query<int>(query).ToList<int>();
                List<Product> listProduct = new List<Product>();
                if (listProductID == null || listProductID.Count <= 0)
                {
                    return null;
                }
                for (int i = 0; i < listProductID.Count; i++)
                {
                    Product product = productService.GetByPrimaryKey(listProductID[i]);
                    listProduct.Add(product);
                }
                return listProduct;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        /// <summary>
        /// get list collection by productID
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public List<Collection> GetCollectionByProductID(int productID)
        {
            try
            {
                string query = "select CollectionID from CollectionProduct where ProductID = " + productID;
                List<int> listCollectionID = connect.Query<int>(query).ToList<int>();
                List<Collection> listCollection = new List<Collection>();
                if (listCollectionID == null || listCollectionID.Count <= 0)
                {
                    return null;
                }
                for (int i = 0; i < listCollectionID.Count; i++)
                {
                    Collection product = collectionService.GetByPrimaryKey(listCollectionID[i]);
                    listCollection.Add(product);
                }
                return listCollection;
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return null;
            }
        }

        /// <summary>
        /// insert a record to table
        /// </summary>
        /// <param name="collectionProduct"></param>
        /// <returns></returns>
        public int Insert(CollectionProduct collectionProduct)
        {
            try
            {
                string query = "insert into CollectionProduct (ProductID,CollectionID) values (@ProductID,@CollectionID) select @@IDENTITY";
                int temp = connect.Query<int>(query, new { collectionProduct.ProductID, collectionProduct.CollectionID }).FirstOrDefault<int>();
                return SNumber.ToNumber(temp);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return 0;
            }
        }

        /// <summary>
        /// delte from table by collectionProductID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteByPrimary(int id)
        {
            try
            {
                if (SNumber.ToNumber(id) <= 0)
                {
                    return false;
                }
                string query = "delete from CollectionProduct where ID = " + id;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        /// <summary>
        /// delete all by collectionID
        /// </summary>
        /// <param name="collectionID"></param>
        /// <returns></returns>
        public bool DeleteByCollectionID(int collectionID)
        {
            try
            {
                if (SNumber.ToNumber(collectionID) <= 0)
                {
                    return false;
                }
                string query = "delete from CollectionProduct where CollectionID = " + collectionID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        /// <summary>
        /// delete all by productID
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public bool DeleteByProductID(int productID)
        {
            try
            {
                if (SNumber.ToNumber(productID) <= 0)
                {
                    return false;
                }
                string query = "delete from CollectionProduct where ProductID = " + productID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }

        /// <summary>
        /// delete by collectionID and productID
        /// </summary>
        /// <param name="collectionID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public bool DeleteByCollectionIDProductID(int collectionID, int productID)
        {
            try
            {
                if (SNumber.ToNumber(collectionID) <= 0)
                {
                    return false;
                }
                string query = "delete from CollectionProduct where CollectionID = " + collectionID + " and ProductID = " + productID;
                return 0 < connect.Execute(query);
            }
            catch (Exception ex)
            {
                LogService.WriteException(ex);
                return false;
            }
        }
    }
}