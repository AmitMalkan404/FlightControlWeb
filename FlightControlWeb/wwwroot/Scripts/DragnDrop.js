const dropArea = document.getElementById('drop-area');

function preventDefaults(e) {
    e.preventDefault();
    e.stopPropagation();
}

['dragenter', 'dragover', 'dragleave', 'drop'].forEach((eventName) => {
    dropArea.addEventListener(eventName, preventDefaults, false);
});

// Highlights the drop area
function highlight() {
    dropArea.classList.add('highlight');
}

//// Stops highlighting the drop area
function unhighlight() {
    dropArea.classList.remove('highlight');
}

['dragenter', 'dragover'].forEach((eventName) => {
    dropArea.addEventListener(eventName, highlight, false);
});

['dragleave', 'drop'].forEach((eventName) => {
    dropArea.addEventListener(eventName, unhighlight, false);
});

/* eslint-disable no-undef */
/* eslint-disable no-prototype-builtins */
// Uploads file to server.
function uploadFile(file) {
    //const url = 'https://localhost:5001/api/FlightPlan';
    const url = '/api/FlightPlan';
    try {
        (async () => {
            const rawResponse = await fetch(url, {
                method: 'POST',
                body: file
            });
            if (rawResponse.ok) {
                console.log('no errors');
                return;
            }
            // Handle errors returned from server.
            else {
                const jsonContent = await rawResponse.json();
                for (const err in jsonContent.errors) {
                    if (jsonContent.errors.hasOwnProperty(err)) {
                        // console.log(data.errors[err].toString());
                        PostErrorNotification(jsonContent.errors[err].toString());
                    }
                }
            }
        })();
    } catch (err) {
        console.log('error: ' + err.message);
    }
}
/* eslint-enable no-prototype-builtins */
/* eslint-enable no-undef */

function handleFiles(files) {
    ([...files]).forEach(uploadFile);
}

function handleDrop(e) {
    const dt = e.dataTransfer;
    const { files } = dt;
    handleFiles(files);
}

dropArea.addEventListener('drop', handleDrop, false);
