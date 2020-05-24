﻿//let allMyFlightsUrl = "https://localhost:5001/api/Flights?relative_to=2020-12-27T02:00:30Z&sync_all";
let d = new Date();
let utcString = d.toUTCString();
let time = new Date(utcString);
let rowIndex = 0;
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
            "</td>" +
            "<td>" +
            "Remove".bold() +
            "</td></tr>");
        $("#externalFlightstable").html('');
        $("#externalFlightstable").append("<tr><td>" +
            "Flight_Id".bold() +
            "</td>" +
            "<td>" +
            "Airline".bold() +
            "</td>" +
            "<td>" +
            "Remove".bold() +
            "</td></tr>");
        data.forEach(function (flight) {
            let isExternal = flight.isExternal;
            if (!isExternal) {
                $("#myflightstable").append("<tr><td>" +
                    flight.flight_id +
                    "</td>" +
                    "<td>" +
                    flight.company_name +
                    "</td>" +
                    "<td>" +
                    "<button class='removeFlightBtn' name='removeFlightBtn' class='btn btn-default'><img src='Pictures/remove.png' width='25px' height='25px'></button>" + 
                    "</td></tr>");
            } else {
                $("#externalFlightstable").append("<tr><td>" +
                    flight.flight_id +
                    "</td>" +
                    "<td>" +
                    flight.company_name +
                    "</td>" +
                    "<td>" +
                    "<button class='removeFlightBtn' name='removeFlightBtn' class='btn btn-default'><img src='Pictures/remove.png' width='25px' height='25px'></button>" +
                    "</td></tr>");
            }
            rowIndex++;
        });
    });
}

$("#myflightstable").on("click", "removeFlightBtn", function (e) {
    $(this).closest("tr").remove();
    //needs to send a command to the server to remove the flight from DB, it's refreshing & adding it again
});
$("#externalFlightstable").on("click", "removeFlightBtn", function (e) {
    $(this).closest("tr").remove();
});
setInterval(getAllFlights, 500);
