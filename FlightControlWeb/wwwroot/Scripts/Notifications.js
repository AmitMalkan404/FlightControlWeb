
let lastError = null;
// Array of errors that we want to show the user if they happen.
const errorsArray = [
    'Latitude has to be between -90.000001 to 90', 'Longitude has to be between -180.000001 to 180',
    'TimeSpan must be possitive', 'Flight plan has to have a company_name',
    'Amount of passengers must be possitive', 'A flight has to have a have route'
];

// Shows error to the client via notify.js
/* eslint-disable no-unused-vars */
function PostErrorNotification(data) {
    if (errorsArray.includes(data)) {
        $.notify(data, 'error');
    } else if (lastError === null) {
        $.notify('There has been a problem with your Json file', 'error');
        lastError = 'There has been a problem with your Json file';
        setTimeout(() => { lastError = null; }, 1000);
    }
}
/* eslint-enable no-unused-vars */
