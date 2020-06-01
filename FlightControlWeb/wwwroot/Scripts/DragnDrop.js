const dropArea = document.getElementById('drop-area');

function preventDefaults(e) {
    e.preventDefault();
    e.stopPropagation();
}

['dragenter', 'dragover', 'dragleave', 'drop'].forEach((eventName) => {
    dropArea.addEventListener(eventName, preventDefaults, false);
});


function highlight() {
    dropArea.classList.add('highlight');
}

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
            } else {
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

    //fetch(url,
    //    {
    //        method: 'POST',
    //        body: file
    //    })
    //    .then((response) => response.json())
    //    .then((data) => {
    //        for (const err in data.errors) {
    //            if (data.errors.hasOwnProperty(err)) {
    //                // console.log(data.errors[err].toString());
    //                PostErrorNotification(data.errors[err].toString());
    //            }
    //        }
    //    })
    //    .catch((err) => {console.log(err);});
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
