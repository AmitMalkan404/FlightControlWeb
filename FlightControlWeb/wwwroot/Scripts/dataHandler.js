
const flightsArray = [];
let myFlightPlanUrl;
// let latlngs = new Array();
const track = new Array(2).fill(null);
const details = new Array(2).fill(null);
details[0] = document.getElementById('flightDetails');

/* eslint-disable no-undef */
function generateTrack(flightId) {
    if (track[0] !== null) {
        return;
    }
    let i;
    const latlngs = [];
    myFlightPlanUrl = `api/FlightPlan/${flightId}`;
    //myFlightPlanUrl = 'api/FlightPlan/85678h';

    $.ajax({
        url: myFlightPlanUrl,
        dataType: 'json',
        success: function(data) {
            for (i = 0; i < data.segments.length; i += 1) {
                latlngs[i] = [data.segments[i].latitude, data.segments[i].longitude];
            }
            track[0] = mapLine(latlngs);
            track[1] = flightId;
        },
        error: function() {
            $.notify('Error:  Flight ID not found', 'error');
        }
    });
}

function removeTrack() {
    if (track[0] !== null) {
        removeMapLine(track[0]);
        track[0] = null;
        track[1] = null;
    }
}

function generateFlightDetails(flight) {
    if (details[1] === null) {
        details[1] = flight;
        const row = details[0].insertRow(1);
        const flightId = row.insertCell(0);
        flightId.innerText = flight.id;
        const airline = row.insertCell(1);
        airline.innerText = flight.airline;
        const passengers = row.insertCell(2);
        passengers.innerText = flight.passengers;
        const lat = row.insertCell(3);
        lat.innerText = flight.latitude.toFixed(6);
        const lon = row.insertCell(4);
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
    const deleteFlightUrl = `api/Flights/${flightToDelete.id}`;
    // const deleteFlightUrl = 'api/Flights/fgch5';
    // let rowIndex = deleteButton.parentNode.parentNode.rowIndex;
    const { rowIndex } = deleteButton.parentNode;

    fetch(deleteFlightUrl,
            {
                method: 'DELETE',
            })
        .then((response) => { /* Done. Inform the user */
            if (!response.ok) {
                throw Error('err');
            }
            // delete row from flights table
            document.getElementById('myflightstable').deleteRow(rowIndex);
            removeMarkerFromMap(flightToDelete.planeMarker);

            // remove flight details
            if (details[1] !== null && details[1].id === flightToDelete.id) {
                removeFlightDetails();
            }

            // remove track if it belongs to this flight
            if (track[1] !== null && track[1] === flightToDelete.id) {
                removeTrack();
            }

            const flightIndex = flightsArray.findIndex((flight) => flight.id === flightToDelete.id);
            flightsArray.splice(flightIndex, 1);
            console.log('flight deleted successfully');
        })
        .catch(() => { /* Error. Inform the user */
            console.log($.notify('Error: The flight was not deleted from server'));
        });
}

function addRowToTable(tableId, flight, jsonFlight) {
    flight.tableRow = document.createElement('TR');

    flight.tableRow.style.cursor = 'pointer';
    flight.tableRow.style.height = '25px';

    // flight.tableRow.addEventListener("click", function () {
    //    onMarkerClick(flight.planeMarker);
    // }, false);

    document.getElementById(tableId).appendChild(flight.tableRow);

    const flightId = flight.tableRow.insertCell(0);
    flightId.innerText = jsonFlight.flight_id;

    flightId.addEventListener('click', () => {
        onMarkerClick(flight.planeMarker);
    }, false);

    const airline = flight.tableRow.insertCell(1);
    airline.innerText = jsonFlight.company_name;

    airline.addEventListener('click', () => {
        onMarkerClick(flight.planeMarker);
    }, false);

    if (tableId === 'myflightstable') {
        const btn = document.createElement('button');
        btn.style.backgroundColor = 'transparent';
        btn.style.border = 'transparent';

        const btnImage = document.createElement('img');
        btnImage.setAttribute('src', 'Pictures/remove.png');
        btnImage.setAttribute('width', 25);
        btnImage.setAttribute('height', 25);

        // btn.onclick = function() {
        // deleteFlight(this, flight);
        // return false;
        // }
        btn.appendChild(btnImage);
        const removeCell = flight.tableRow.insertCell(2);
        removeCell.appendChild(btn);
        removeCell.onclick = function temp() {
            deleteFlight(this, flight);
            return false;
        };
        removeCell.style.backgroundColor = 'red';
    }
}

function updateFlightDetails() {
    if (details[1] !== null) {
        details[0].rows[1].cells[3].innerText = details[1].planeMarker._latlng.lat.toFixed(6);
        details[0].rows[1].cells[4].innerText = details[1].planeMarker._latlng.lng.toFixed(6);
    }
}

function removeExpiredFlight(j) {
    let tableId;
    if (flightsArray[j].isExternal === false) {
        tableId = 'myflightstable';
    } else {
        tableId = 'externalFlightstable';
    }
    document.getElementById(tableId).deleteRow(flightsArray[j].tableRow.rowIndex);
    removeMarkerFromMap(flightsArray[j].planeMarker);

    // remove flight details
    if (details[1] != null && details[1].id === flightsArray[j].id) {
        removeFlightDetails();
    }

    // remove track if it belongs to this flight
    if (track[1] !== null && track[1] === flightsArray[j].id) {
        removeTrack();
    }
    flightsArray.splice(j, 1);
}

function updateExistingFlights(data) {
    let j;
    let active;
    if (flightsArray.length === 0) {
        return;
    }

    for (j = 0; j < flightsArray.length; j += 1) {
        active = false;

        for (const jsonFlight of data) {
            if (flightsArray[j].id === jsonFlight.flight_id) {
                // the flight is active
                active = true;
                break;
            }
        }

        if (!active) {
            removeExpiredFlight(j);
        }
    }
}

//////////////////////////////////////////////////////////////////////////////////////// ==0 return
/* eslint-disable no-unused-vars */
function linkRowDetailsTrack(marker, action) {
    if (flightsArray.length === 0) {
        console.log('flightsArray is Empty!');
        return;
    }
    if (flightsArray.some(myFlight => myFlight.planeMarker === marker)) {
        const flightIndex = flightsArray.findIndex((myFlight) => myFlight.planeMarker === marker);
        if (action === 'link') {
            flightsArray[flightIndex].tableRow.style.backgroundColor = 'lightgreen';
            generateFlightDetails(flightsArray[flightIndex]);
            generateTrack(flightsArray[flightIndex].id);
        } else {
            flightsArray[flightIndex].tableRow.style.backgroundColor = 'transparent';
            removeFlightDetails();
            removeTrack();
        }
    }

}
/* eslint-enable no-unused-vars */
function getFlights() {
    $.ajax({
        url: allMyFlightsUrl,
        dataType: 'json',
        success: function (data) {
            for (const jsonFlight of data) {
                if (!flightsArray.some(myFlight => myFlight.id === jsonFlight.flight_id)) {
                    const newFlight = new Flight(jsonFlight.flight_id, jsonFlight.latitude,
                        jsonFlight.longitude, jsonFlight.company_name, jsonFlight.passengers);

                    if (jsonFlight.is_external === false) {
                        newFlight.isExternal = false;
                        addRowToTable('myflightstable', newFlight, jsonFlight);
                    } else {
                        newFlight.isExternal = true;
                        addRowToTable('externalFlightstable', newFlight, jsonFlight);
                    }
                    flightsArray.push(newFlight);
                }

                const newFlightIndex = flightsArray.findIndex((myFlight) => myFlight.id === jsonFlight.flight_id);
                if (!flightsArray[newFlightIndex].iconExists) {
                    flightsArray[newFlightIndex].setPlaneMarker(addAirplaneIconToMap(jsonFlight.latitude,
                        jsonFlight.longitude));
                    flightsArray[newFlightIndex].iconExists = true;
                }
                moveMarker(flightsArray[newFlightIndex].getPlaneMarker(), jsonFlight.latitude, jsonFlight.longitude);
                updateFlightDetails();
            }
            updateExistingFlights(data);
        },
        error: function () {
            $.notify('Server is not responding', 'error');
        }
    });

}
/* eslint-enable no-undef */

setInterval(getFlights, 600);
