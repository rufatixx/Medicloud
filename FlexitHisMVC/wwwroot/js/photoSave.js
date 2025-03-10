//var $image = $('#image');
//var cropper;

//function createFileUploadInstance(inputId, formId) {
//	$(`#${inputId}`).on('change', function (event) {
//		var reader = new FileReader();
//		reader.onload = function (event) {
//			// Yüklenen resmin URL'sini ayarla
//			$image.attr('src', event.target.result);

//			// Eğer daha önce bir cropper varsa, onu kaldır
//			if (cropper) {
//				cropper.destroy();
//			}

//			// Yeni Cropper.js nesnesi oluştur
//			cropper = new Cropper($image[0], {
//				aspectRatio: 1, // Oranını belirleyin (1:1 kare)
//				viewMode: 1,    // Görünüm modu
//				autoCropArea: 0.65, // Otomatik kırpma alanı
//				responsive: true,
//				zoomable: true,
//				rotatable: true
//			});
//		};

//		reader.readAsDataURL(this.files[0]);
//	});
//	var canvas;

//	// Kırpılan resmin canvas'ını al
//	if (cropper) {
//		canvas = cropper.getCroppedCanvas();

//		$('#croppedImage').attr('src', canvas.toDataURL());
//	}


//	$('#savePhotoButton').on('click', function () {
//		var canvas = cropper.getCroppedCanvas();
//		var imageData = canvas.toDataURL();
//		var reader = new FileReader();
//		reader.onload = function (event) {
//			// Yüklenen resmin URL'sini ayarla
//			$image.attr('src', event.target.result);

//			// Eğer daha önce bir cropper varsa, onu kaldır
//			if (cropper) {
//				cropper.destroy();
//			}

//			// Yeni Cropper.js nesnesi oluştur
//			cropper = new Cropper($image[0], {
//				aspectRatio: 1, // Oranını belirleyin (1:1 kare)
//				viewMode: 1,    // Görünüm modu
//				autoCropArea: 0.65, // Otomatik kırpma alanı
//				responsive: true,
//				zoomable: true,
//				rotatable: true
//			});
//		};

//		var croppedFile= reader.readAsDataURL(this.files[0]);

//		$(`#${inputId}`).val(croppedFile)
//		$(`#${formId}`).submit();
//	});

//}



//$('#resetButton').on('click', function () {
//	if (cropper) {
//		cropper.reset();
//	}
//});


var cropper;
var currentInputId;
var currentFormId;
var currentOutputId;

function openModal(inputId, formId,outputId) {
	// Modal'ı aç

	$('#photoModal').modal('show');
	currentInputId = inputId;
	currentFormId = formId;
	currentOutputId = outputId;
}

function createFileUploadInstance(inputId, formId,outputId) {
	$('#' + inputId).on('change', function (event) {
		var reader = new FileReader();
		reader.onload = function (e) {
			// Yüklenen resmin URL'sini modalda görüntülemek
			$('#image').attr('src', e.target.result);

			// Eğer daha önce bir cropper varsa, onu kaldır
			if (cropper) {
				cropper.destroy();
			}

			// Yeni Cropper.js nesnesi oluştur
			//cropper = new Cropper($('#image')[0], {
			//	aspectRatio: NaN,    // Oranını belirleyin (1:1 kare)
			//	viewMode:1,       // Görünüm modu
			//	autoCropArea: 1, // Otomatik kırpma alanı
			//	responsive: false,
			//	zoomable: false,
			//	rotatable: true
			//});
			var img = $('#image')[0];
			cropper = new Cropper(img, {
				aspectRatio: 4/3,    // Allow free cropping (set to NaN if you don't want a fixed aspect ratio)
				viewMode: 1,         // Set the view mode (1: within the container)
				autoCropArea: 1,     // This will make the cropper fill the entire image area by default
				responsive: false,    // Allow responsive resizing of the cropper
				zoomable: false,     // Disable zoom
				rotatable: false,     // Enable image rotation
				background: false,   // Disable background (default is false, can be omitted)
				movable: true,       // Allow movement of the cropper within the image
				ready: function () {
					// Adjust the cropper size to match the image's width (or your preferred width)
					var imageWidth = img.width;
					cropper.setCropBoxData({
						width: imageWidth,  // Set crop box width to the image's width
						height: imageWidth  // You can adjust this as needed
					});
					$(".cropper-container").css("background-color", "white"); // Example: Dark transparent background

					// Make the crop box circular
					//$(".cropper-view-box").css({
					//	"border-radius": "50%",  // Circular crop box
					//	"border": "2px solid #fff" // Optional white border
					//});
				}
			});

			// Modal'ı aç
			openModal(inputId, formId, outputId);
		};

		reader.readAsDataURL(this.files[0]);
	});
}

// Reset button'ı ile cropper'ı sıfırlama
$('#resetButton').on('click', function () {
	if (cropper) {
		cropper.reset();
	}
});

// Kaydet butonu ile resmi kaydetme ve formu gönderme
$('#savePhotoButton').on('click', function () {
	var canvas;

	// Kırpılan resmin canvas'ını al
	if (cropper) {
		canvas = cropper.getCroppedCanvas();

		var imageData = canvas.toDataURL();



		var imageType = imageData.split(';')[0].split(':')[1];

		// Convert the base64 image data to a Blob
		var byteString = atob(imageData.split(',')[1]);
		var arrayBuffer = new ArrayBuffer(byteString.length);
		var uintArray = new Uint8Array(arrayBuffer);

		for (var i = 0; i < byteString.length; i++) {
			uintArray[i] = byteString.charCodeAt(i);
		}

		// Create a Blob from the byte array with the appropriate MIME type
		var blob = new Blob([arrayBuffer], { type: imageType });

		// Create a new File object from the Blob
		var file = new File([blob], "cropped_image." + imageType.split('/')[1], { type: imageType });

		// Append the file to the 'logoPhoto' file input
		var fileInput = $('#' + currentOutputId)[0];
		var dataTransfer = new DataTransfer(); // Create a new DataTransfer object
		dataTransfer.items.add(file);  // Add the file to the DataTransfer object
		fileInput.files = dataTransfer.files; // Assign the files to the input


		$('#' + currentFormId).submit();

		$('#photoModal').modal('hide');
	}
}



)

$('#photoModal').on('shown.bs.modal', function () {
	if (cropper) {
		cropper.resize();
		cropper.setCropBoxData({ left: 0, top: 0 });
	}
});

$('#photoModal').on('hidden.bs.modal', function () {

	if (cropper) {
		cropper.destroy();
		cropper = null;  
	}


	$('#image').attr('src', '');  


	$('#' + currentInputId).val(''); 

});