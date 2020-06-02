// Initialize map.
const mymap = L.map('mapid').setView([32.08, 34.78], 2);
L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token={accessToken}', {
    attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors, <a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
    maxZoom: 18,
    minZoom: 1,
    id: 'mapbox/streets-v11',
    tileSize: 512,
    zoomOffset: -1,
    accessToken: 'pk.eyJ1IjoicGlrYWNodTIzIiwiYSI6ImNrYTJramNkMDAydWEzZnA5M2l5Zmdnc28ifQ.kw350v1vaUcIs6r6oz8p2g'
}).addTo(mymap);

// Sets the unselected plane icon.
const unselectedIcon = L.icon({
    iconUrl: 'Pictures/unselectedIcon.png',
    iconSize: [55, 55],
    iconAnchor: [0, 0]
});

// Sets the selected plane icon.
const selectedIcon = L.icon({
    iconUrl: 'Pictures/selectedIcon.png',
    iconSize: [70, 70],
    iconAnchor: [0, 0]
});

// Global flags for click event.
let isMarkerClicked = false;
let clickedMarker = null;

/* eslint-disable no-undef */
// The set of actions to do when we don't want to select a flight.
function releaseClick() {
    if (isMarkerClicked) {
        clickedMarker.setIcon(unselectedIcon);
        linkRowDetailsTrack(clickedMarker, 'unLink');
        isMarkerClicked = false;
    }
}

// Release the selected flight in case of clicking the map.
function onMapClick() {
    releaseClick();
}

// The set of actions to do when we want to select a flight.
function onMarkerClick(marker) {
    if (isMarkerClicked) {
        releaseClick();
    }
    marker.setIcon(selectedIcon);
    isMarkerClicked = true;
    clickedMarker = marker;
    linkRowDetailsTrack(marker, 'link');
}

// Sets a new location for the marker,
/* eslint-disable no-unused-vars */
function moveMarker(marker, lat, lon) {
    const newLatLng = new L.LatLng(lat, lon);
    marker.setLatLng(newLatLng);
    // marker.setLatLng(newLatLng).update();
}

// Adds an airplane marker to the map.
function addAirplaneIconToMap(latitude, longitude) {
    const marker = new L.Marker([latitude, longitude], { icon: unselectedIcon });
    marker.addEventListener('click', () => {
        onMarkerClick(marker);
    }, false);
    mymap.addLayer(marker);
    return marker;
}

// Removes an airplane marker from the map.
function removeMarkerFromMap(marker) {
    mymap.removeLayer(marker);
}

// Creates a new line on the map (plane track).
function mapLine(latlngs) {
    const polyline = new L.polyline(latlngs, { color: 'red' });
    mymap.addLayer(polyline);
    return polyline;
}

// Removes a line from the map (plane track).
function removeMapLine(line) {
    mymap.removeLayer(line);
}
/* eslint-enable no-unused-vars */

// Defines a click event for the map.
mymap.on('click', onMapClick);
/* eslint-enable no-undef */

