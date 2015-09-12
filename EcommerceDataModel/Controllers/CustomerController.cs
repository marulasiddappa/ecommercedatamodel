using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EcommerceDataModel.Models;
using System.Data.SqlClient;

namespace EcommerceDataModel.Controllers
{
    public class CustomerController : ApiController
    {
        private ECommerceEntities db = new ECommerceEntities();

        // GET api/Customer/5
        [ResponseType(typeof(List<CustomerModel>))]
        [Route("api/customer/{query}")]
        public IHttpActionResult GetCustomer(string query)
        {
            string q = "select * from Customer c where Contains((FirstName,LastName,Email,PhoneNo,City), @query)";
            List<CustomerModel> customers = db.Database.SqlQuery<CustomerModel>(q, new SqlParameter("@query", query)).ToList();
            return Ok(customers);
        }

        [ResponseType(typeof(CustmerViewModel))]
        [Route("api/customer/{id:int}")]
        public IHttpActionResult GetCustomer(int id)
        {
            var model = GetCustomerModel(id);
            if (model == null)
                return NotFound();
            return Ok(model);
        }

        [ResponseType(typeof(CustmerViewModel))]
        [Route("api/customer/{id}/{query}")]
        public IHttpActionResult GetCustomer(int id, string query)
        {
            var model = GetCustomerModel(id, query);
            if (model == null)
                return NotFound();
            return Ok(model);
        }

        public CustmerViewModel GetCustomerModel(int id, string filter = null)
        {
            CustmerViewModel model = new CustmerViewModel();
            var c = db.Customers.Find(id);
            if (c == null)
            {
                return null;
            }
            var customerDetails = (db.CustomerDetails
                .Where(cd => cd.CustomerID == id && (string.IsNullOrEmpty(filter) || cd.MetaColumn.MetaTable.Name == filter))
                .Select(cd => cd));

            model.Customer = new CustomerModel
            {
                ID = c.ID,
                City = c.City,
                Email = c.Email,
                FirstName = c.FirstName,
                LastName = c.LastName,
                PhoneNo = c.PhoneNo
            };

            foreach (var d in customerDetails)
            {
                model.Detail.Add(new CustomerCustomData
                {
                    FieldName = d.MetaColumn.Name,
                    FieldValue = d.FieldValue,
                    TableName = d.MetaColumn.MetaTable.Name,
                    FieldID = d.MetaColumnID,
                    TableID = d.MetaColumn.MetaTableID
                });
            }
            return model;
        }


        // PUT api/Customer/5
        //public IHttpActionResult PutCustomer(int id, Customer customer)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != customer.ID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(customer).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!CustomerExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST api/Customer
        [ResponseType(typeof(CustmerViewModel))]
        public IHttpActionResult PostCustomer(CustmerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var c = model.Customer;
            Customer customer = new Customer
            {
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                PhoneNo = c.PhoneNo,
                City = c.City,

            };
            db.Customers.Add(customer);
            foreach (var detail in model.Detail)
            {
                CustomerDetail d = new CustomerDetail
                {
                    Customer = customer,
                    MetaColumnID = detail.FieldID,
                    FieldValue = detail.FieldValue
                };
                db.CustomerDetails.Add(d);
            }
            db.SaveChanges();
            return CreatedAtRoute("DefaultApi", new { id = model.Customer.ID }, model);
        }

        //// DELETE api/Customer/5
        //[ResponseType(typeof(Customer))]
        //public IHttpActionResult DeleteCustomer(int id)
        //{
        //    Customer customer = db.Customers.Find(id);
        //    if (customer == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Customers.Remove(customer);
        //    db.SaveChanges();

        //    return Ok(customer);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //private bool CustomerExists(int id)
        //{
        //    return db.Customers.Count(e => e.ID == id) > 0;
        //}
    }
}