
let lastError = null;

/* eslint-disable no-unused-vars */
function PostErrorNotification(data) {
    // const options = {
    //    // whether to hide the notification on click
    //    clickToHide: true,
    //    // whether to auto-hide the notification
    //    autoHide: true,
    //    // if autoHide, hide after milliseconds
    //    autoHideDelay: 5000,
    //    // show the arrow pointing at the element
    //    arrowShow: true,
    //    // arrow size in pixels
    //    arrowSize: 5,
    //    // position defines the notification position though uses the defaults below
    //    position: '...',
    //    // default positions
    //    elementPosition: 'bottom left',
    //    globalPosition: 'top right',
    //    // default style
    //    style: 'bootstrap',
    //    // default class (string or [string])
    //    className: 'error',
    //    // show animation
    //    showAnimation: 'slideDown',
    //    // show animation duration
    //    showDuration: 400,
    //    // hide animation
    //    hideAnimation: 'slideUp',
    //    // hide animation duration
    //    hideDuration: 200,
    //    // padding between element and notification
    //    gap: 2
    // }

    if (data === 'Latitude has to be between -90.000001 to 90'
        || data === 'Longitude has to be between -180.000001 to 180'
        || data == 'TimeSpan must be possitive') {
        $.notify(data, 'error');
    } else if (lastError === null) {
        $.notify('There has been a problem with your Json file', 'error');
        lastError = 'There has been a problem with your Json file';
        setTimeout(() => { lastError = null; }, 1000);
    }
}
/* eslint-enable no-unused-vars */
