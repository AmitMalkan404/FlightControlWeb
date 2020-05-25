class Flight {
    constructor(id, lat, lon, company, passengers) {
        this.id = id;
        this.latitude = lat;
        this.longitude = lon;
        this.iconExists = false;
        this.airline = company;
        this.planeMarker = null;
        this.track = null;
        this.tableRow = null;
        this.isExternal = null;
        this.passengers = passengers;
    }

    setPlaneMarker(marker) {
        this.planeMarker = marker;
    }

    getPlaneMarker() {
        return this.planeMarker;
    }

    setPlaneTrack(planeTrack) {
        this.track = planeTrack;
    }

    setMyFlightsRow(row) {
        this.tableRow = row;
    }

    getPlaneTrack() {
        return this.track;
    }

    getMyFlightsRow() {
        return this.tableRow;
    }

    getId() {
        return this.id;
    }

    getLatitude() {
        return this.latitude;
    }

    getLongitude() {
        return this.longitude;
    }
}

function isInFlightsArray(flight) {
    for (i = 0; i < flightsArray.length; i++) {
        if (flight.flight_id === flightsArray[i].id) {
            return true;
        }
    }
    return false;
}

let flightsArray = new Array();
let myFlightPlanUrl; 
let latlngs = new Array();
let flightDetails = false;

function getlatlngs(flightsArray, arrayIndex) {
    myFlightPlanUrl = "api/FlightPlans/" + flightsArray[arrayIndex].id;
    
   $.getJSON(myFlightPlanUrl, function (data) {
        for (i = 0; i < data.segments.length; i++) {
            latlngs[i] = new Array(data.segments[i].latitude, data.segments[i].longitude);
        }
        flightsArray[arrayIndex].setPlaneTrack(mapLine(latlngs));

        //console.log(flightsArray[arrayIndex].getPlaneTrack());

        
        //flightsArray[arrayIndex].setPlaneTrack = latlngs;
    });

    //console.log(flightsArray[arrayIndex].getPlaneTrack());
    //mapLine(latlngs);

}


function linkRowAndDetails(marker, action) {
    if (flightsArray.length !== 0) {
        let i;
        for (i = 0; i < flightsArray.length; i++) {
            if (flightsArray[i].planeMarker === marker) {

                let tableId;
                if (flightsArray[i].isExternal === true) {
                    tableId = "externalFlightstable";
                } else {
                    tableId = "myflightstable";
                }

                if (action === "link") {
                    flightsArray[i].tableRow.style.backgroundColor = "lightgreen";
                    generateFlightDetails(flightsArray[i]);
                } else {
                    flightsArray[i].tableRow.style.backgroundColor = "transparent";
                    removeFlightDetails();
                }
            }
        }
    }
}


let i = 0;

function getFlights() {

    $.getJSON(allMyFlightsUrl, function (data) {
        //console.log(data);
        data.forEach(function (jsonFlight) {
            if (!isInFlightsArray(jsonFlight)) {
                flightsArray[i] = new Flight(jsonFlight.flight_id, jsonFlight.latitude, jsonFlight.longitude,
                    jsonFlight.company_name, jsonFlight.passengers);

                if (jsonFlight.is_external === false) {
                    flightsArray[i].isExternal = false;
                    addRowToTable("myflightstable", flightsArray[i], jsonFlight);
                } else {
                    flightsArray[i].isExternal = true;
                    addRowToTable("externalFlightstable", flightsArray[i], jsonFlight);
                }

                //addRowToTable("flightDetails", flightsArray[i], jsonFlight);
            }
            if (!flightsArray[i].iconExists) {
                flightsArray[i].setPlaneMarker(addAirplaneIconToMap(jsonFlight.latitude, jsonFlight.longitude));
                flightsArray[i].iconExists = true;
            }
            moveMarker(flightsArray[i].getPlaneMarker(), jsonFlight.latitude, jsonFlight.longitude);
            //getlatlngs(flightsArray, i);

            i++;
        });
    });
}

function addRowToTable(tableId, flight, jsonFlight) {

    flight.tableRow = document.createElement("TR");

    flight.tableRow.style.cursor = "pointer";
    flight.tableRow.style.height = "25px";

    //flight.tableRow.addEventListener("click", function () {
    //    onMarkerClick(flight.planeMarker);
    //}, false);

    document.getElementById(tableId).appendChild(flight.tableRow);

    let flightId = flight.tableRow.insertCell(0);
    flightId.innerText = jsonFlight.flight_id;
    ////////
    flightId.addEventListener("click", function () {
        onMarkerClick(flight.planeMarker);
    }, false);
    ////////
    let airline = flight.tableRow.insertCell(1);
    airline.innerText = jsonFlight.company_name;
    ////////
    airline.addEventListener("click", function () {
        onMarkerClick(flight.planeMarker);
    }, false);
    ////////
    if (tableId === "myflightstable") {
        let btn = document.createElement('button');
        btn.style.backgroundColor = "transparent";
        btn.style.border = "transparent";

        let btnImage = document.createElement('img');
        btnImage.setAttribute("src", "Pictures/remove.png");
        btnImage.setAttribute("width", 25);
        btnImage.setAttribute("height", 25);

        //btn.onclick = function() {
        //    deleteFlight(this, flight);
        //    return false;
        //}
        btn.appendChild(btnImage);
        let removeCell = flight.tableRow.insertCell(2);
        removeCell.appendChild(btn);
        removeCell.onclick = function () {
            deleteFlight(this, flight);
            return false;
        }
        removeCell.style.backgroundColor = "red";
    }
}

function generateFlightDetails(flight) {
    if (!flightDetails) {
        let row = document.createElement("TR");
        document.getElementById("flightDetails").appendChild(row);
        let flightId = row.insertCell(0);
        flightId.innerText = flight.id;
        let airline = row.insertCell(1);
        airline.innerText = flight.airline;
        let passengers = row.insertCell(2);
        passengers.innerText = flight.passengers;
        let lat = row.insertCell(3);
        lat.innerText = flight.latitude;
        let lon = row.insertCell(4);
        lon.innerText = flight.longitude;
        flightDetails = true;
    }
}

function removeFlightDetails() {
    if (flightDetails) {
        document.getElementById("flightDetails").deleteRow(1);
        flightDetails = false;
    }
}

function deleteFlight(deleteButton, flightToDelete) {

    let deleteFlightUrl = "api/Flights/" + flightToDelete.id;

    //let rowIndex = deleteButton.parentNode.parentNode.rowIndex;
    let rowIndex = deleteButton.parentNode.rowIndex;

    fetch(deleteFlightUrl,
        {
            method: 'DELETE',
        })

        .then(() => { /* Done. Inform the user */
            //delete row from flights table
            document.getElementById("myflightstable").deleteRow(rowIndex);
            removeMarkerFromMap(flightToDelete.planeMarker);

            //remove flight details if they belong to this flight
            let detailsTable = document.getElementById("flightDetails");
            if (detailsTable.rows.length > 1 && detailsTable.rows[1].cells[0].innerText === flightToDelete.id) {
                removeFlightDetails();
            }

            console.log("flight deleted successfully");
        })
        .catch(() => { /* Error. Inform the user */
            console.log("Error Deleting flight");
        });
}


setInterval(getFlights, 3000);