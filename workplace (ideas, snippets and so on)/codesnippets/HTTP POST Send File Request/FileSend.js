(function jsLoaded() {
    console.log("js is loaded!")
})();

document.getElementById("fileForm").addEventListener("submit", function (event) {
    event.preventDefault();

    let fileInput = document.getElementById("FileInput");

    if (fileInput.files.length > 0) {
        let file = fileInput.files[0];
        let formData = new FormData();

        if (isValidFileType(file)) {
            formData.append("userFile", file)
            console.log("Form Data:");
            for (let pair of formData.entries()) {
                console.log(pair[0], pair[1]);
            }
        }
        else {
            alert("Please submit a jpeg file, other file types are not allowed.")
        }
    }
})

function isValidFileType(file) {
    const allowedTypes = ["image/jpeg"];
    return allowedTypes.includes(file.type);
}

// Send POST request

/*
let xhr = new XMLHttpRequest();
    xhr.open('POST', 'your-api-endpoint-url', true);
    xhr.onreadystatechange = function() {
      if (xhr.readyState === 4 && xhr.status === 200) {
        // Request successful, handle the response
        let response = JSON.parse(xhr.responseText);
        console.log(response);
      }
    };
    xhr.send(formData);
  }
});
*/