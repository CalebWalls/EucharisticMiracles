using System;
using System.Text.Json;
using EucharisticMiracles.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EucharisticMiracles.Data;

namespace EucharisticMiracles.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocationFinderController : ControllerBase
    {

        private readonly ILogger<LocationFinderController> _logger;
        private readonly UserContext _userDbContext;

        public LocationFinderController(ILogger<LocationFinderController> logger, UserContext userDbContext)
        {
            _logger = logger;
            _userDbContext = userDbContext;
        }

        [HttpPost("LocationFinder")]
        public async Task<LocationOfMiracle> LocationFinder([FromBody] LocationRequest request, CancellationToken cancellationToken)
        {
            var userLocation = new FindLoc(_userDbContext);
            return await userLocation.FindLocation(request, cancellationToken);
        }

        [HttpPost("ListOfLocations")]
        public async Task<List<LocationOfMiracle>> ListOfAllMiracles(CancellationToken cancellationToken)
        {
            var locations = await _userDbContext.MiracleLocations
               .Select(x => new { x.Street, x.City, x.State, x.Zip, x.Latitude, x.Longitude, x.NameOfLocation })
               .ToListAsync(cancellationToken);

            List<LocationOfMiracle>? mappedLocations = new List<LocationOfMiracle>();
            foreach (var location in locations)
            {
                mappedLocations.Add(
                    new LocationOfMiracle
                    {
                        LocationName = location.NameOfLocation,
                        State = location.State,
                        Street = location.Street,
                        City = location.City,
                        Zip = location.Zip,
                        Latitude = location.Latitude,
                        Longitude = location.Longitude
                    });
            }

            return mappedLocations;
        }

        [HttpPost("AddLocations")]
        public async Task<IActionResult> AddLocations([FromBody] MiracleLocations request, CancellationToken cancellationToken)
        {
            //TODO:Let only admin
            var result = _userDbContext.Add(request);
            await result.Context.SaveChangesAsync(cancellationToken);

            return Ok();
        }


    }
}








