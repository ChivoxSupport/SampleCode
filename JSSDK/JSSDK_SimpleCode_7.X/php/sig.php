<?php
//AppKey provided by Chivox
$appKey="your appKey"; 
//SecretKey provided by Chivox
$secretKey="your secretKey";
//user Id
$userId="your userId";

//Timestamp
$timestamp = floor(microtime(1)*1000);
//Put the appKey, secretKey, and timestamp into a character array
$tokens=array($appKey,$timestamp,$secretKey);

$alg="sha256";
//Convert the sorted character array into a string in order
$string="";
foreach ($tokens as $token) {
$string=$string.$token;
}
//Use SHA-256 encryption on the sorted string
$signature = hash('sha256', $string);
//Assign appKey, timestamp, and signature to an object
$obj=new stdClass();
$obj->applicationId=$appKey;
$obj->timestamp=(string)$timestamp;
$obj->sig=$signature;
$obj->alg=$alg;
$obj->userId=$userId;

//Set the HTTP response Content-type to application/json
header('Content-type: application/json');
//Convert the object into JSON data
echo json_encode($obj);
?>
