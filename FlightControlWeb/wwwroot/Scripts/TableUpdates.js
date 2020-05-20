//let allMyFlightsUrl = "https://localhost:5001/api/Flights?relative_to=2020-12-27T02:00:30Z&sync_all";
//$.getJSON(allMyFlightsUrl, function (data) {
//    data.forEach(function (flight) {
//            ("#myflightstable").append("<tr><td>" +
//                flight.FlightId +
//                "</td>" +
//                "<td>" +
//                flight.CompanyName +
//                "</td></tr>" >
//            );
//        });
//    });

//let allMyFlightsUrl = "https://localhost:5001/api/Flights?relative_to=2020-12-27T02:00:30Z&sync_all";
//$.getJSON(allMyFlightsUrl, function(data) {
//    data.forEach(function(flight) {
//        $("#myflightstable").append("<tr><td>") +
//            flight.FlightId +
//            "</td>" +
//            "<td>" +
//            flight.CompanyName +
//            "</td></tr>";
//    });
//});

//const flightsBody = document.querySelector("#myflightstable")


let allMyFlightsUrl = "https://localhost:5001/api/Flights?relative_to=2020-12-27T02:00:30Z&sync_all";

function getAllFlights() {
    $.getJSON(allMyFlightsUrl, function (data) {
        $("#myflightstable").html('');
        $("#myflightstable").append("<tr><td>" +
            "Flight_Id".bold() +
            "</td>" +
            "<td>" +
            "Airline".bold() +
            "</td></tr>");
        $("#externalFlightstable").html('');
        $("#externalFlightstable").append("<tr><td>" +
            "Flight_Id".bold() +
            "</td>" +
            "<td>" +
            "Airline".bold() +
            "</td></tr>");
        data.forEach(function (flight) {
            let isExternal = flight.isExternal;
            if (!isExternal) {
                $("#myflightstable").append("<tr><td>" +
                    flight.flight_id +
                    "</td>" +
                    "<td>" +
                    flight.company_name +
                    "</td></tr>");
            } else {
                $("#externalFlightstable").append("<tr><td>" +
                    flight.flight_id +
                    "</td>" +
                    "<td>" +
                    flight.company_name +
                    "</td></tr>");
            }

            //console.log(flight.FlightId);
            //console.log(flight.CompanyName);
            //console.log(typeof JSON.stringify(flight.FlightId));

        });
    });
}

setInterval(getAllFlights, 3000);
