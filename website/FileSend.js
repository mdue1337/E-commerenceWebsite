document.getElementById("fileForm").addEventListener("submit", function (event) {
    event.preventDefault();

    let fileInput = document.getElementById("FileInput");

    let sellerId = document.getElementById("SellerId").value
    let productName = document.getElementById("Name").value
    let price = document.getElementById("Price").value
    let color = document.getElementById("Color").value
    let brand = document.getElementById("Brand").value

    let url = `https://localhost:7159/Shop/CreateListingClothes?SellerId=${sellerId}&Name=${productName}&Price=${price}&Color=${color}&Brand=${brand}`;


    if (fileInput.files.length > 0) {
        let file = fileInput.files[0];
        let formData = new FormData();

        if (isValidFileType(file)) {
            formData.append("picture", file)
            
            let sizes = document.getElementById("sizes").value
            let sizesArray = sizes.split(' ')

            formData.append("sizes", sizesArray)

            console.log(sizesArray)
            
            let xhr = new XMLHttpRequest();
            xhr.open('POST', url, true);
            xhr.onreadystatechange = function () {
                if (xhr.readyState === 4 && xhr.status === 200) {
                    let response = JSON.parse(xhr.responseText);
                    console.log(response)
                }
            };
            xhr.send(formData);
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