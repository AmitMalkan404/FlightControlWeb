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

function uploadFile(file) {
    const url = 'https://localhost:5001/api/FlightPlans';
    // let formData = new FormData()

    // formData.append('file', file)

    fetch(url,
        {
            method: 'POST',
            body: file,
        })
        .then((response) => {
            if (response.ok) {
                console.log('file updated on server side');
                return;
            }
            console.log(response.fail);

            throw new Error(response.fail);
        })
        .catch((error) => {
            console.log(`Request failed: ${error.message}`);
        });

    // .then((response) => {
    //    if (response.status === 200) { // Or what ever you want to check
    //        return Promise.resolve(response.json()); // This will end up in SUCCESS part
    //    }
    //    console.log(response);
    //    console.log(response.json());

    //    return Promise.resolve(response.json()).then((responseInJson) => {
    //            return Promise.reject(responseInJson.message);
    //        });
    // })
    // .then((result) => { // SUCCESS part
    //        console.log(`Success: ${result}`); // Response from api in json
    //    },
    //    (error) => { // ERROR part
    //        // Because we rejected responseInJson.message,
    //        // error will contain message from api.In this case "Some nasty error message!"
    //        console.log(`Error: ${error}`);
    //    })
    // .catch((catchError) => {
    //    console.log(`Catch: ${catchError}`);
    // });


    // .catch(() => { /* Error. Inform the user */
    //    console.log();
    // });
}

function handleFiles(files) {
    ([...files]).forEach(uploadFile);
}

function handleDrop(e) {
    const dt = e.dataTransfer;
    const { files } = dt;
    handleFiles(files);
}

dropArea.addEventListener('drop', handleDrop, false);
