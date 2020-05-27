let dropArea = document.getElementById('drop-area');

['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
    dropArea.addEventListener(eventName, preventDefaults, false);
});

function preventDefaults(e) {
    e.preventDefault();
    e.stopPropagation();
}

;['dragenter', 'dragover'].forEach(eventName => {
        dropArea.addEventListener(eventName, highlight, false);
});


['dragleave', 'drop'].forEach(eventName => {
    dropArea.addEventListener(eventName, unhighlight, false);
});

function highlight(e) {
    dropArea.classList.add('highlight')
}

function unhighlight(e) {
    dropArea.classList.remove('highlight')
}

dropArea.addEventListener('drop', handleDrop, false)

function handleDrop(e) {
    let dt = e.dataTransfer;
    let files = dt.files;
    handleFiles(files);
}

function handleFiles(files) {
    ([...files]).forEach(uploadFile);
}

function uploadFile(file) {
    let url = 'https://localhost:5001/api/FlightPlans';
    //let formData = new FormData()

    //formData.append('file', file)

    fetch(url,
            {
                method: 'POST',
                body: file
        })
    .then((response) => {
            if (response.status === 200) { // Or what ever you want to check
                return Promise.resolve(response.json()); // This will end up in SUCCESS part
        }
        console.log(response);
        console.log(response.json());
            return Promise.resolve(response.json()).then((responseInJson) => { // This will end up in ERROR part
                return Promise.reject(responseInJson.message); //  responseInJson.message = "Some nasty error message!"
            });
        })
        .then((result) => { // SUCCESS part
                console.log("Success: ", result); // Response from api in json
            },
            (error) => { // ERROR part
                // Because we rejected responseInJson.message, error will contain message from api. In this case "Some nasty error message!"
                console.log("Error: ", error);
            })
        .catch(catchError => {
            console.log("Catch: ", catchError);
        });


    ////Here i should use the server or services!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //.then(function (response) {                      // first then()
    //    if (response.ok) {
    //        console.log("file updated on server side");
    //        return;
    //    }
    //    console.log(response.fail);

    //    throw new Error(response.fail);

    //})
    //.catch(function (error) {                        // catch
    //    console.log('Request failed', error.message);
    //});


    //.catch(() => { /* Error. Inform the user */
    //    console.log();
    //});
}