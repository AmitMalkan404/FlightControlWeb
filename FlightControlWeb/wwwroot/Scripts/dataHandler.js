class Flight {
    constructor(id, lat, lon, company, passengers) {
        this.id = id;
        this.latitude = lat;
        this.longitude = lon;
        this.iconExists = false;
        this.airline = company;
        this.planeMarker = null;
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
//let latlngs = new Array();
let track = new Array(2).fill(null);
let details = new Array(2).fill(null);
details[0] = document.getElementById("flightDetails");

function generateTrack(flightId) {
    if (track[0] === null) {
        let latlngs = new Array();
        myFlightPlanUrl = "api/FlightPlans/" + flightId;

        $.getJSON(myFlightPlanUrl, function (data) {
            for (i = 0; i < data.segments.length; i++) {
                latlngs[i] = new Array(data.segments[i].latitude, data.segments[i].longitude);
            }
            track[0] = mapLine(latlngs);
            track[1] = flightId;
        });
    }
}

function removeTrack() {
    if (track[0] !== null) {
        removeMapLine(track[0]);
        track[0] = null;
        track[1] = null;
    }
}


function linkRowDetailsTrack(marker, action) {
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
                    generateTrack(flightsArray[i].id);
                    //updateFlightDetails(flightsArray[i]);

                } else {
                    flightsArray[i].tableRow.style.backgroundColor = "transparent";
                    removeFlightDetails();
                    removeTrack();
                }
            }
        }
    }
}


let i = 0;

function getFlights() {

    $.getJSON(allMyFlightsUrl, function (data) {
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
            }

            if (!flightsArray[i].iconExists) {
                flightsArray[i].setPlaneMarker(addAirplaneIconToMap(jsonFlight.latitude, jsonFlight.longitude));
                flightsArray[i].iconExists = true;
            }
            console.log(jsonFlight.latitude, jsonFlight.longitude);
            moveMarker(flightsArray[i].getPlaneMarker(), jsonFlight.latitude, jsonFlight.longitude);
            updateFlightDetails();
            i++;
        });
        updateExistingFlights(data, flightsArray);
    });
}


function updateExistingFlights(data, flightsArray) {
    let j;
    let active;
    if (flightsArray.length === 0) {
        return;
    }

    for (j = 0; j < flightsArray.length; j++) {
        active = false;

        for (let jsonFlight of data) {
            if (flightsArray[j].id === jsonFlight.flight_id) {
                // the flight is active
                active = true;
                break;
            }
        }

        if (!active) {
            removeExpiredFlight(flightsArray, j);
        }
    }
}


function removeExpiredFlight(flightsArray, j) {
    let tableId;
    if (flightsArray[j].isExternal === false) {
        tableId = "myflightstable";
        //document.getElementById("myflightstable").deleteRow(
        //    flightsArray[j].tableRow.rowIndex);
    }
    else {
        tableId = "externalFlightstable";
        //document.getElementById("externalFlightstable").deleteRow(
        //    flightsArray[j].tableRow.rowIndex);
    }
    document.getElementById(tableId).deleteRow(flightsArray[j].tableRow.rowIndex);
    removeMarkerFromMap(flightsArray[j].planeMarker);

    //remove flight details
    if (details[1] != null && details[1].id === flightsArray[j].id) {
        removeFlightDetails();
    }

    //remove track if it belongs to this flight
    if (track[1] !== null && track[1] === flightsArray[j].id) {
        removeTrack();
    }
    flightsArray.splice(j, 1);
}


function updateFlightDetails() {
    if (details[1] !== null) {
        details[0].rows[1].cells[3].innerText = details[1].planeMarker._latlng.lat.toFixed(6);
        details[0].rows[1].cells[4].innerText = details[1].planeMarker._latlng.lng.toFixed(6);
    }
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
    if (details[1] === null) {
        details[1] = flight;
        let row = details[0].insertRow(1);
        let flightId = row.insertCell(0);
        flightId.innerText = flight.id;
        let airline = row.insertCell(1);
        airline.innerText = flight.airline;
        let passengers = row.insertCell(2);
        passengers.innerText = flight.passengers;
        let lat = row.insertCell(3);
        lat.innerText = flight.latitude.toFixed(6);
        let lon = row.insertCell(4);
        lon.innerText = flight.longitude.toFixed(6);
    }
}

function removeFlightDetails() {
    if (details[1] !== null) {
        details[0].deleteRow(1);
        details[1] = null;
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

            //remove flight details
            if (details[1] !== null && details[1].id === flightToDelete.id) {
                removeFlightDetails();
            }

            //remove track if it belongs to this flight
            if (track[1] !== null && track[1] === flightToDelete.id) {
                removeTrack();
            }

            let flightIndex = flightsArray.findIndex(flight => flight.id === flightToDelete.id);
            flightsArray.splice(flightIndex, 1);
            console.log("flight deleted successfully");
        })
        .catch(() => { /* Error. Inform the user */
            console.log("Error Deleting flight");
        });
}


setInterval(getFlights, 500);