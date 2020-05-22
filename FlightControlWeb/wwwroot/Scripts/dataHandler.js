class Flight {
    constructor(id, lat, lon, iconId) {
        this.id = id;
        this.latitude = lat;
        this.longitude = lon;
        this.iconExists = false;
        this.exists = false;
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


let myFlightPlanUrl; 

function getlatlngs(id, arrayIndex) {
    myFlightPlanUrl = "api/FlightPlans/" + id;
    let latlngs = new Array();
    $.getJSON(myFlightPlanUrl, function (data) {
        for (i = 0; i < data.segments.length; i++) {
            latlngs[i] = new Array(data.segments[i].latitude, data.segments[i].longitude);
        }
        flightsArray[arrayIndex].setPlaneTrack = latlngs;
    });
    mapLine(latlngs);
}


let flightsArray = new Array();

function getFlights() {
    let i = 0;
    
    $.getJSON(allMyFlightsUrl, function (data) {
        
        data.forEach(function (flight) {
            if (!isInJsonArray(flight)) {
                flightsArray[i] = new Flight(flight.flight_id, flight.latitude, flight.longitude);
                if (flightsArray[i].iconExists) {
                    map.removeLayer(flightsArray[i].planeIcon);
                }
                flightsArray[i].setPlaneMarker = addAirplaneIconToMap(flight.latitude, flight.longitude);
                flightsArray[i].iconExists = true;
                getlatlngs(flightsArray[i].id, i);
                //flightsArray[i].setPlaneTrack = mapLine(getlatlngs(flightsArray[i].id));
                i++;
            }
        });
    });
}


setInterval(getFlights, 2000);