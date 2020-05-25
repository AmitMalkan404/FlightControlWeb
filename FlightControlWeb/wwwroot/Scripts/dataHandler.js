class Flight {
    constructor(id, lat, lon, company) {
        this.id = id;
        this.latitude = lat;
        this.longitude = lon;
        this.iconExists = false;
        this.airline = company;
        this.planeMarker = null;
        this.track = null;
        this.tableRow = null;
        this.isExternal = null;
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


function paintTableRow(marker, action) {
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

                if (action === "mark") {
                    flightsArray[i].tableRow.style.backgroundColor = "red";
                } else {
                    flightsArray[i].tableRow.style.backgroundColor = "transparent";
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
                flightsArray[i] = new Flight(jsonFlight.flight_id, jsonFlight.latitude, jsonFlight.longitude, jsonFlight.company_name);
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

function func(flight) {
    //flight.planeMarker.fireEvent('click');
}

function addRowToTable(tableId, flight, jsonFlight) {

    flight.tableRow = document.createElement("TR");

    //flight.tableRow.addEventListener("click", markerClick, flight.planeMarker);
    flight.tableRow.addEventListener("click", function () {
        markerClick(flight.planeMarker);
    }, false);

    document.getElementById(tableId).appendChild(flight.tableRow);

    let flightId = flight.tableRow.insertCell(0);
    flightId.innerText = jsonFlight.flight_id;
    let airline = flight.tableRow.insertCell(1);
    airline.innerText = jsonFlight.company_name;

    if (tableId === "myflightstable") {
        let btn = document.createElement('button');
        let btnImage = document.createElement('img');
        btnImage.setAttribute("src", "Pictures/remove.png");
        btnImage.setAttribute("width", 25);
        btnImage.setAttribute("height", 25);

        btn.onclick = function() {
            deleteFlight(this, flight);
            return false;
        }

        btn.appendChild(btnImage);
        let removeCell = flight.tableRow.insertCell(2);
        removeCell.appendChild(btn);
    }
    //else if (tableId === "flightDetails") {
    //    let passengers = flight.tableRow.insertCell(2);
    //    passengers.innerText = jsonFlight.passengers;
    //    let lat = flight.tableRow.insertCell(3);
    //    lat.innerHTML = jsonFlight.latitude;
    //    let lon = flight.tableRow.insertCell(4);
    //    lon.innerHTML = jsonFlight.longitude;
    //}
}


function deleteFlight(deleteButton, flightToDelete) {

    let deleteFlightUrl = "api/Flights/" + flightToDelete.id;

    let rowIndex = deleteButton.parentNode.parentNode.rowIndex;

    fetch(deleteFlightUrl,
        {
            method: 'DELETE',
        })

        .then(() => { /* Done. Inform the user */
            //delete row from flights table
            document.getElementById("myflightstable").deleteRow(rowIndex);
            removeMarkerFromMap(flightToDelete.planeMarker);
            //removeMapLine(flightToDelete.getPlaneTrack());

            console.log("flight deleted successfully");
        })
        .catch(() => { /* Error. Inform the user */
            console.log("Error Deleting flight");
        });
}


setInterval(getFlights, 3000);