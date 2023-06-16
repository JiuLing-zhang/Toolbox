window.GetTextMD5 = (input) => {
    let value = CryptoJS.MD5(input).toString();
    return value;
};

window.GetFileMD5 = (input) => {
    let buffer = CryptoJS.lib.WordArray.create(input);
    let value = CryptoJS.MD5(buffer).toString();
    return value;
};

window.GetTextSHA1 = (input) => {
    let value = CryptoJS.SHA1(input).toString();
    return value;
};

window.GetFileSHA1 = (input) => {
    let buffer = CryptoJS.lib.WordArray.create(input);
    let value = CryptoJS.SHA1(buffer).toString();
    return value;
};

window.GetTextSHA256 = (input) => {
    let value = CryptoJS.SHA256(input).toString();
    return value;
};

window.GetFileSHA256 = (input) => {
    let buffer = CryptoJS.lib.WordArray.create(input);
    let value = CryptoJS.SHA256(buffer).toString();
    return value;
};