document.getElementById("fileForm").addEventListener("submit", function (event) {
    event.preventDefault();

    let fileInput = document.getElementById("FileInput");

    let url = `https://localhost:7159/Shop/CreateListingClothes?SellerId=${SellerId}&Name=${Name}&Price=${Price}&Color=${Color}&Brand=${Brand}`;

    if (fileInput.files.length > 0) {
        let file = fileInput.files[0];
        let formData = new FormData();

        if (isValidFileType(file)) {
            let sizes = document.getElementById("sizes").value;
            let sizesArray = sizes.split(' ');

            let data = {
                SellerId: document.getElementById("SellerId").value,
                Name: document.getElementById("Name").value,
                Price: parseFloat(document.getElementById("Price").value),
                Color: document.getElementById("Color").value,
                Brand: document.getElementById("Brand").value,
            }

            formData.append("data", JSON.stringify(data));
            formData.append("Picture", file);
            formData.append("sizes", sizesArray)

            fetch(url, {
                method: 'PUT',
                body: formData,
            })
            .then(response => response.json())
            .then(updatedListing => {
                console.log('Updated listing:', updatedListing);
            })
            .catch(error => {
                console.error('Error updating listing:', error);
            });
        }
    }
    else {
        alert("Please submit a jpeg file, other file types are not allowed.");
    }
});

function isValidFileType(file) {
    const allowedTypes = ["image/jpeg"];
    return allowedTypes.includes(file.type);
}



/*
document.getElementById("fileForm").addEventListener("submit", function (event) {
    event.preventDefault();

    let fileInput = document.getElementById("FileInput");

    let url = `https://localhost:7159/Shop/CreateListingClothes?SellerId=${SellerId}&Name=${Name}&Price=${Price}&Color=${Color}&Brand=${Brand}`;

    if (fileInput.files.length > 0) {
        let file = fileInput.files[0];
        let formData = new FormData();

        if (isValidFileType(file)) {
            let sizes = document.getElementById("sizes").value
            let sizesArray = sizes.split(' ')

            let data = {
                SellerId: document.getElementById("SellerId").value,
                Name: document.getElementById("Name").value,
                Price: parseFloat(document.getElementById("Price").value),
                Color: document.getElementById("Color").value,
                Brand: document.getElementById("Brand").value,
            }

            formData.append("data", JSON.stringify(data))
            formData.append("picture", file)
            formData.append("sizes", sizesArray)

            fetch(url, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data),
            })
                .then(response => response.json())
                .then(updatedListing => {
                    console.log('Updated listing:', updatedListing);
                })
                .catch(error => {
                    console.error('Error updating listing:', error);
                });
        }
    }
    else {
        alert("Please submit a jpeg file, other file types are not allowed.")
    }
}
)

function isValidFileType(file) {
    const allowedTypes = ["image/jpeg"];
    return allowedTypes.includes(file.type);
}
*/