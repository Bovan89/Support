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
    public class ClientRequestController : BaseWebAPIController
    {
        // GET: api/ClientRequest
        public IQueryable GetClientRequest()
        {
            var reqs = (from req in db.ClientRequest
                        join empl in db.Employee on req.Executor equals empl into gj
                        from subemp in gj.DefaultIfEmpty()
                        select new
                        {
                            ID = req.ID,
                            CreatedOn = req.CreatedOn,
                            ProcessTime = req.ProcessTime,
                            StartTime = req.StartTime,
                            State = req.State,
                            Text = req.Text,
                            Executor = subemp
                        }
                         ).OrderByDescending(r => r.CreatedOn);

            return reqs;            
        }

        // GET: api/ClientRequest
        public IQueryable GetClientRequest(ClientRequest.ClientRequestState requestState, bool withThisState = true)
        {
            var reqs = (from req in db.ClientRequest
                        where !(withThisState ^ req.State == requestState)
                        join empl in db.Employee on req.Executor equals empl into gj
                        from subemp in gj.DefaultIfEmpty()
                        select new
                        {
                            ID = req.ID,
                            CreatedOn = req.CreatedOn,
                            ProcessTime = req.ProcessTime,
                            StartTime = req.StartTime,
                            State = req.State,
                            Text = req.Text,
                            Executor = subemp
                        }
                        ).OrderByDescending(r => r.CreatedOn);

            return reqs;
        }

        // GET: api/ClientRequest/5
        [ResponseType(typeof(ClientRequest))]
        public IHttpActionResult GetClientRequest(int id)
        {
            ClientRequest clientRequest = db.ClientRequest.Find(id);
            if (clientRequest == null)
            {
                return NotFound();
            }            

            return Ok(clientRequest);
        }

        // PUT: api/ClientRequest/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutClientRequest(int id, ClientRequest clientRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != clientRequest.ID)
            {
                return BadRequest();
            }

            db.Entry(clientRequest).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientRequestExists(id))
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

        // POST: api/ClientRequest
        [ResponseType(typeof(ClientRequest))]
        public IHttpActionResult PostClientRequest(ClientRequest clientRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                        
            clientRequest.CreatedOn = DateTime.Now;
            clientRequest.Insert(db);            

            return CreatedAtRoute("DefaultApi", new { id = clientRequest.ID }, clientRequest);
        }

        // DELETE: api/ClientRequest/5
        [ResponseType(typeof(ClientRequest))]
        public IHttpActionResult DeleteClientRequest(int id)
        {
            ClientRequest clientRequest = db.ClientRequest.Find(id);
            if (clientRequest == null)
            {
                return NotFound();
            }

            if (clientRequest.State == ClientRequest.ClientRequestState.Complete)
            {
                return Conflict();
            }

            if (clientRequest.State != ClientRequest.ClientRequestState.Cancel)
            {
                clientRequest.Cancel(db);
            }
            
            //Db.ClientRequest.Remove(clientRequest);

            return Ok(clientRequest);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClientRequestExists(int id)
        {
            return db.ClientRequest.Count(e => e.ID == id) > 0;
        }
    }
}