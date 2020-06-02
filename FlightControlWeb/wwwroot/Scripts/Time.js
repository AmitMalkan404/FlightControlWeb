﻿// A global url for get flights api
/* eslint-disable no-unused-vars */
let allMyFlightsUrl;
/* eslint-enable no-unused-vars */

// Updates the system's time in UTC
function getTime() {
    const d = new Date();
    const utcString = d.toUTCString();
    let time = new Date(utcString);
    time = time.toISOString().slice(0, -5);
    allMyFlightsUrl = `/api/Flights?relative_to=${time}Z&sync_all`;

}

setInterval(getTime, 500);
