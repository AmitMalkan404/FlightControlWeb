const dropArea = document.getElementById('drop-area');

function preventDefaults(e) {
    e.preventDefault();
    e.stopPropagation();
}

['dragenter', 'dragover', 'dragleave', 'drop'].forEach((eventName) => {
    dropArea.addEventListener(eventName, preventDefaults, false);
});

/* eslint-disable no-unused-vars */
function highlight(e) {
    dropArea.classList.add('highlight');
}

function unhighlight(e) {
    dropArea.classList.remove('highlight');
}
/* eslint-enable no-unused-vars */
['dragenter', 'dragover'].forEach((eventName) => {
    dropArea.addEventListener(eventName, highlight, false);
});

['dragleave', 'drop'].forEach((eventName) => {
    dropArea.addEventListener(eventName, unhighlight, false);
});

/* eslint-disable no-prototype-builtins */
function uploadFile(file) {
    const url = 'https://localhost:5001/api/FlightPlans';

    fetch(url,
        {
            method: 'POST',
            body: file
        })
        .then((response) => response.json())
        .then((data) => {
            for (const err in data.errors) {
                if (data.errors.hasOwnProperty(err)) {
                    // console.log(data.errors[err].toString());
                    PostErrorNotification(data.errors[err].toString());
                }
            }
        })
        .catch((err) => {console.log(err);});
}
/* eslint-enable no-prototype-builtins */

function handleFiles(files) {
    ([...files]).forEach(uploadFile);
}

function handleDrop(e) {
    const dt = e.dataTransfer;
    const { files } = dt;
    handleFiles(files);
}

dropArea.addEventListener('drop', handleDrop, false);
