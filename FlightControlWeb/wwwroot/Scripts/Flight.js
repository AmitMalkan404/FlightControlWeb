// Define Flight class
/* eslint-disable no-unused-vars */
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
}
/* eslint-enable no-unused-vars */