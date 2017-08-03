using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ODataMovies.Models;
using ODataMovies.Business;
using System.Web.OData;
using System.Web.Http;
using System.Net;
using System.Diagnostics;

namespace ODataMovies.Controllers
{
    public class MoviesController : ODataController
    {
        private DataService m_service = new DataService();
        [EnableQuery]
        //Find below sample urls
        //http://localhost:32097/api/Movies?$top=1
        //http://localhost:32097/api/Movies?$filter=Id eq 1
        //http://localhost:32097/api/Movies?$filterId lt 3
        //http://localhost:32097/api/Movies?$filter=Id ge 3 and Id le 5
        //http://localhost:32097/api/Movies?$orderby=Title desc
        //http://localhost:32097/api/Movies?$orderby=Title asc
        //http://localhost:32097/api/Movies?$orderby=Id asc&$top=5
        //http://localhost:32097/api/Movies?$top=5&$skip=3
        //http://localhost:32097/api/Movies?$orderby=Title asc &$skip=6
        public IList<Movie> Get()
        {
			return m_service.Movies;
        }
		[HttpGet]
        //Find below sample urls
        //http://localhost:32097/api/Movies(1)
        public Movie Get(int key)
        {
            IEnumerable<Movie> movie = m_service.Movies.Where(m => m.Id == key);
            if (movie.Count() == 0)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            else
                return movie.FirstOrDefault();
		}

		
		public IHttpActionResult Post([FromBody] Movie movie)
        {
            try
            {
                return Ok<Movie>(m_service.Add(movie));
            }
            catch(ArgumentNullException e)
            {
                Debugger.Log(1, "Error", e.Message);
				return BadRequest();
            }
            catch(ArgumentException e)
            {
                Debugger.Log(1, "Error", e.Message);
				return BadRequest();
			}
            catch(InvalidOperationException e)
            {
                Debugger.Log(1, "Error", e.Message);
				return Conflict();
            }
        }

        public IHttpActionResult Put(int key, Movie movie)
        {
            try
            {
                movie.Id = key;
                return Ok<Movie>(m_service.Save(movie));
            }
            catch(ArgumentNullException)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            catch(ArgumentException)
            {
				return NotFound();
            }
        }

       
        public IHttpActionResult Delete(int key)
        {
            if (m_service.Remove(key))
                return Ok();
            else
                return NotFound();            
        }

       
        public IHttpActionResult Patch(int key, Delta<Movie> moviePatch)
        {
            Movie movie = m_service.Find(key);
            if (movie == null) return NotFound();
            moviePatch.CopyChangedValues(movie);
            return Ok<Movie>(m_service.Save(movie));
        }

        
    }
}