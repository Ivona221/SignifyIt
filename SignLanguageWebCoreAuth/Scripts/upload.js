$(document).ready(function () {

	document.getElementById('uploader').onsubmit = function () {
		var formdata = new FormData(); //FormData object
		var fileInput = document.getElementById('fileInput');
		//Iterating through each files selected in fileInput
		for (i = 0; i < fileInput.files.length; i++) {
			//Appending each file to FormData object
			formdata.append(fileInput.files[i].name, fileInput.files[i]);
		}
		//Creating an XMLHttpRequest and sending
		var xhr = new XMLHttpRequest();
		xhr.open('POST', '/Home/Upload');
		xhr.send(formdata);
		xhr.onreadystatechange = function () {
			if (xhr.readyState == 4 && xhr.status == 200) {
				alert(xhr.responseText);
			}
		}
		return false;
	}   
});