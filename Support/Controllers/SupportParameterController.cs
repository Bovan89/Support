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
using Support.DAL;
using Support.Models;

namespace Support.Controllers
{
    public class SupportParameterController : BaseWebAPIController
    {
        // GET: api/SupportParameter
        public IQueryable<SupportParameter> GetSupportParameter()
        {
            return db.SupportParameter;
        }

        // GET: api/SupportParameter/5
        [ResponseType(typeof(SupportParameter))]
        public IHttpActionResult GetSupportParameter(int id)
        {
            SupportParameter supportParameter = db.SupportParameter.Find(id);
            if (supportParameter == null)
            {
                return NotFound();
            }

            return Ok(supportParameter);
        }

        // PUT: api/SupportParameter/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSupportParameter(int id, SupportParameter supportParameter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SupportParameter parameter = db.SupportParameter.Find(id);
            if (parameter == null)
            {
                return NotFound();
            }

            parameter.ParameterValue = supportParameter.ParameterValue;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupportParameterExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/SupportParameter
        [ResponseType(typeof(SupportParameter))]
        public IHttpActionResult PostSupportParameter(SupportParameter supportParameter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SupportParameter.Add(supportParameter);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = supportParameter.ID }, supportParameter);
        }

        // DELETE: api/SupportParameter/5
        [ResponseType(typeof(SupportParameter))]
        public IHttpActionResult DeleteSupportParameter(int id)
        {
            SupportParameter supportParameter = db.SupportParameter.Find(id);
            if (supportParameter == null)
            {
                return NotFound();
            }

            db.SupportParameter.Remove(supportParameter);
            db.SaveChanges();

            return Ok(supportParameter);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupportParameterExists(int id)
        {
            return db.SupportParameter.Count(e => e.ID == id) > 0;
        }
    }
}