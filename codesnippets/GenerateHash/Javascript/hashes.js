var Hashes = require('jshashes')

document.getElementById("userPassword").addEventListener("submit", function (event) {
    event.preventDefault();

    let pw = document.getElementById("UserPw");

    var pwHash = new Hashes.SHA1().b64(pw);

    document.getElementById("hash").innerText = pwHash;
});

// browserify hashes.js -o browserhash.js