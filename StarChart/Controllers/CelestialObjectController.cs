﻿using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if(celestialObject== null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context.CelestialObjects.Where(x=>x.OrbitedObjectId == id).ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(x=>x.Name.Equals(name));

            if (celestialObjects.Count() == 0)
                return NotFound();

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = celestialObjects.Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject) 
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CelestialObject celestialObject)
        {
            var _celestialObject = _context.CelestialObjects.Find(id);
            if (_celestialObject == null)
                return NotFound();

            _celestialObject.Name = celestialObject.Name;
            _celestialObject.OrbitalPeriod = celestialObject.OrbitalPeriod;

            _context.CelestialObjects.Update(_celestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name) 
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject == null)
                return NotFound();

            celestialObject.Name = name;

            _context.CelestialObjects.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(x=>x.Id == id || x.OrbitedObjectId == id);

            if (celestialObjects.Count() == 0)
                return NotFound();

            _context.RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
