class Flight {
    constructor(id, lat, lon, company) {
        this.id = id;
        this.latitude = lat;
        this.longitude = lon;
        this.iconExists = false;
        this.airline = company;
        this.planeMarker = null;
        this.track = null;
        this.myFlightsRow = null;
        
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
        this.myFlightsRow = row;
    }

    getPlaneTrack() {
        return this.track;
    }

    getMyFlightsRow() {
        return this.myFlightsRow;
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

        console.log(flightsArray[arrayIndex].getPlaneTrack());

        
        //flightsArray[arrayIndex].setPlaneTrack = latlngs;
    });

    console.log(flightsArray[arrayIndex].getPlaneTrack());
    //mapLine(latlngs);

}


let i = 0;

function getFlights() {

    $.getJSON(allMyFlightsUrl, function (data) {
        console.log(data);
        data.forEach(function (jsonFlight) {
            if (!isInFlightsArray(jsonFlight)) {
                flightsArray[i] = new Flight(jsonFlight.flight_id, jsonFlight.latitude, jsonFlight.longitude, jsonFlight.company_name);
                if (jsonFlight.is_external === false) {
                    addRowToTable("myflightstable", flightsArray[i], jsonFlight);
                } else {
                    addRowToTable("externalFlightstable", flightsArray[i], jsonFlight);
                }
                addRowToTable("flightDetails", flightsArray[i], jsonFlight);
            }
            if (!flightsArray[i].iconExists) {
                flightsArray[i].setPlaneMarker(addAirplaneIconToMap(jsonFlight.latitude, jsonFlight.longitude));
                flightsArray[i].iconExists = true;
            }
            moveMarker(flightsArray[i].getPlaneMarker(), jsonFlight.latitude, jsonFlight.longitude);
            getlatlngs(flightsArray, i);

            i++;
        });
    });
}


function addRowToTable(tableId, flight, jsonFlight) {

    flight.myFlightsRow = document.createElement("TR");
    document.getElementById(tableId).appendChild(flight.myFlightsRow);

    let flightId = flight.myFlightsRow.insertCell(0);
    flightId.innerText = jsonFlight.flight_id;
    let airline = flight.myFlightsRow.insertCell(1);
    airline.innerText = jsonFlight.company_name;

    if (tableId === "myflightstable") {
        let btn = document.createElement('button');
        let btnImage = document.createElement('img');
        btnImage.setAttribute("src", "Pictures/remove.png");
        btnImage.setAttribute("width", 25);
        btnImage.setAttribute("height", 25);
        btn.appendChild(btnImage);
        let removeCell = flight.myFlightsRow.insertCell(2);
        removeCell.appendChild(btn);
    }
    else if (tableId === "flightDetails") {
        let passengers = flight.myFlightsRow.insertCell(2);
        passengers.innerText = jsonFlight.passengers;
        let lat = flight.myFlightsRow.insertCell(3);
        lat.innerHTML = jsonFlight.latitude;
        let lon = flight.myFlightsRow.insertCell(4);
        lon.innerHTML = jsonFlight.longitude;
    }
}

//deleteRow(flight.myFlightsRow.rowIndex, tableId);

function deleteRow(rowIndex, tableId) {
    document.getElementById(tableId).deleteRow(rowIndex);
}


setInterval(getFlights, 4000);