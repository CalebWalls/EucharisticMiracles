using EucharisticMiracles.Controllers;
using EucharisticMiracles.Data;
using EucharisticMiracles.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading;

namespace EucharisticMiracles
{
    public class FindLoc
    {
        private readonly UserContext _userDbContext;
        public FindLoc(UserContext userDbContext) 
        {
            _userDbContext = userDbContext;

        }
        public async Task<LocationOfMiracle> FindLocation(LocationRequest request, CancellationToken cancellationToken)
        {
            // Define the 5 locations in the database
            var locations = await _userDbContext.MiracleLocations
                .Select(x => new { x.Street, x.City, x.State, x.Zip, x.Latitude, x.Longitude, x.NameOfLocation, x.VideoLink})
                .ToListAsync(cancellationToken);

            List<LocationOfMiracle>? mappedLocations = new List<LocationOfMiracle>();
            foreach(var location in locations)
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
                        Longitude = location.Longitude,
                        VideoLink = location.VideoLink
                    });
            }

            // Ask the user for their address
            string userAddress = $"{request.Street}, {request.City}, {request.State} {request.Zip}";

            // Geocode the user's address to get its latitude and longitude
            var userLocation = await GeocodeAddress(userAddress);

            // Find the closest location to the user
            return FindClosestLocation(userLocation, mappedLocations);

            // Display the result to the user
            //Console.WriteLine($"The nearest location to you is {closestLocation.Name}, located at {closestLocation.Address}.");
        }

        static async Task<Location> GeocodeAddress(string address)
        {
            string apiKey = "LOCALAPIKEY"; // Replace with your own API key
            string apiUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={apiKey}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);

            var location = json.RootElement.GetProperty("results")[0].GetProperty("geometry").GetProperty("location");
            var latitude = location.GetProperty("lat").GetDouble();
            var longitude = location.GetProperty("lng").GetDouble();

            return new Location
            {
                User = "User",
                Address = address,
                Latitude = latitude,
                Longitude = longitude
            };
        }

        public LocationOfMiracle? FindClosestLocation(Location userLocation, List<LocationOfMiracle> locations)
        {
            double closestDistance = double.MaxValue;
            LocationOfMiracle closestLocation = null;
            double longitude = double.MaxValue;
            double latitude = double.MaxValue;

            foreach (var location in locations)
            {
                double.TryParse(location.Longitude, out longitude);
                double.TryParse(location.Latitude, out latitude);
                double distance = GetDistance(userLocation.Latitude, userLocation.Longitude, latitude, longitude);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestLocation = location;
                }
            }

            return closestLocation;
        }

        public double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // This method uses the haversine formula to calculate the great-circle distance between two points on a sphere (i.e. the Earth)
            // For simplicity, we'll assume that the Earth is a perfect sphere with a radius of 6,371 kilometers
            const double EarthRadius = 6371;

            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = EarthRadius * c;
            return distance;
        }

        public double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
