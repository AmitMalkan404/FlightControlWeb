//let allMyFlightsUrl = "https://localhost:5001/api/Flights?relative_to=2020-12-27T02:00:30Z&sync_all";
let d = new Date();
let utcString = d.toUTCString();
let time = new Date(utcString);
time = time.toISOString().slice(0, -2);

let allMyFlightsUrl = "https://localhost:5001/api/Flights?relative_to=" + time + "Z&sync_all";

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
        });
    });
}

setInterval(getAllFlights, 500);
