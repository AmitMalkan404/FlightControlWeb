let mymap = L.map('mapid').setView([32.08, 34.78], 2);
L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token={accessToken}', {
    attribution: 'Map data &copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors, <a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
    maxZoom: 18,
    minZoom: 1,
    id: 'mapbox/streets-v11',
    tileSize: 512,
    zoomOffset: -1,
    accessToken: 'pk.eyJ1IjoicGlrYWNodTIzIiwiYSI6ImNrYTJramNkMDAydWEzZnA5M2l5Zmdnc28ifQ.kw350v1vaUcIs6r6oz8p2g'
}).addTo(mymap);

//Added by Amit
let unselectedIcon = L.icon({
    iconUrl: "Pictures/unselectedIcon.png",
    iconSize: [55, 55],
    iconAnchor: [0, 0]
});

let selectedIcon = L.icon({
    iconUrl: "Pictures/selectedIcon.png",
    iconSize: [70, 70],
    iconAnchor: [0, 0]
});

let isMarkerClicked = false;
let clickedMarker = null;

//L.marker([19.02, 57.52], { icon: airplaneIcon }).addTo(mymap);
//L.marker([29.72, 35.00], { icon: airplaneIcon }).addTo(mymap);
//moveMarker(markerr, 15.51, 28.00);


function releaseClick() {
    if (isMarkerClicked) {
        clickedMarker.setIcon(unselectedIcon);
        linkRowDetailsTrack(clickedMarker, "unLink");
        isMarkerClicked = false;
    }
}

function onMapClick() {
    releaseClick();
}

function onMarkerClick(marker) {
    if (isMarkerClicked) {
        releaseClick();
    }
    marker.setIcon(selectedIcon);
    isMarkerClicked = true;
    clickedMarker = marker;
    linkRowDetailsTrack(marker, "link");
}

function moveMarker(marker, lat, lon) {

    let newLatLng = new L.LatLng(lat, lon);
    marker.setLatLng(newLatLng);
    //marker.setLatLng(newLatLng).update();
}

function addAirplaneIconToMap(latitude, longitude) {
    let marker = new L.Marker([latitude, longitude], { icon: unselectedIcon });
    //marker.on('click', this.onMarkerClick, this);

    marker.addEventListener("click", function () {
        onMarkerClick(marker);
    }, false);

    mymap.addLayer(marker);
    return marker;
}

function removeMarkerFromMap(marker) {
    mymap.removeLayer(marker);
}

function generatePlaneTrackArray(segment) {
    //returns an array of locations from JSON's segment to build the plane's track.
}

function mapLine(latlngs) {
    let polyline = new L.polyline(latlngs, { color: 'red' });
    mymap.addLayer(polyline);
    //polyline.addTo(mymap);
    return polyline;
}

function removeMapLine(line) {
    console.log(line);
    mymap.removeLayer(line);
}


mymap.on('click', onMapClick);

//let latlng = [[32.01, 34.88], [19.02, 57.52], [13.77, 100.66]];

//let poly = new L.polyline(latlng, { color: 'red' });
//mymap.addLayer(poly);
//mymap.removeLayer(poly);