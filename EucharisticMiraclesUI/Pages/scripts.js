$("#locationFinderForm").submit(function (event) {
    event.preventDefault();
    var requestData = {
        Street: $("#streetInput").val(),
        City: $("#cityInput").val(),
        State: $("#stateInput").val(),
        Zip: $("#zipInput").val()
    };
    $.ajax({
        type: "POST",
        url: "https://localhost:7065/LocationFinder/LocationFinder",
        data: JSON.stringify(requestData),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var locationHtml = '<p><strong>Location Name:</strong> ' + response.locationName + '</p>' +
                '<p><strong>Street:</strong> ' + response.street + '</p>' +
                '<p><strong>City:</strong> ' + response.city + '</p>' +
                '<p><strong>State:</strong> ' + response.state + '</p>' +
                '<p><strong>Zip:</strong> ' + response.zip + '</p>' +
                '<p><strong>Latitude:</strong> ' + response.latitude + '</p>' +
                '<p><strong>Longitude:</strong> ' + response.longitude + '</p>';
            $("#locationFinderResult").html(locationHtml);
        },
        error: function () {
            $("#locationFinderResult").html("Error occurred while finding location.");
        }
    });
});