class Flight {
    constructor(id, lat, lon, iconId) {
        this.id = id;
        this.latitude = lat;
        this.longitude = lon;
        this.iconExists = false;
    }

    setPlaneMarker(marker) {
        this.planeMarker = marker;
    }

    setPlaneTrack(planeTrack) {
        this.track = planeTrack;
    }

    getPlaneTrack() {
        return this.track;
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

function isInJsonArray(flight) {
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
        flightsArray[arrayIndex].setPlaneTrack = mapLine(latlngs);

        console.log(flightsArray[arrayIndex].getPlaneTrack());

        
        //flightsArray[arrayIndex].setPlaneTrack = latlngs;
    });

    console.log(flightsArray[arrayIndex].getPlaneTrack());
    //mapLine(latlngs);

}




function getFlights() {
    let i = 0;
    
    $.getJSON(allMyFlightsUrl, function (data) {
        
        data.forEach(function (flight) {
            if (!isInJsonArray(flight)) {
                flightsArray[i] = new Flight(flight.flight_id, flight.latitude, flight.longitude);
            }
            if (flightsArray[i].iconExists) {
                removeMarkerFromMap(flightsArray[i].planeIcon);
            }
            flightsArray[i].setPlaneMarker = addAirplaneIconToMap(flight.latitude, flight.longitude);
            flightsArray[i].iconExists = true;
            getlatlngs(flightsArray,i);
            //flightsArray[i].setPlaneTrack = mapLine(getlatlngs(flightsArray[i].id));
            i++;
        });
    });
}



function removeFlightByID(id) {
    let z = 0;
    for (z = 0; z < arr.length; z++) {
        if (flightsArray[z].flight_id == id) {
            arr.splice(z, 1);
            z--;
        }
    }
}

function getFlightByID(id) {

}

setInterval(getFlights, 2000);