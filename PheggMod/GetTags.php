<?php

ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);

//This works off the fact that the tag table is made from 5 columns, with the table name being "PlayerBadges"
//SteamID - text
//DiscordID - text
//Prefix - text
//Suffix - text
//Colour - text

$conn = new mysqli("127.0.0.1", "user", "password", "tags");

if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

$result = mysqli_query($conn, "SELECT * FROM PlayerBadges");

if (mysqli_num_rows($result) > 0) {
    $data = array();
    while ($row = mysqli_fetch_array($result)) {

        array_push($data, array(
            'steamID' => $row["SteamID"], 'discordID' => $row["DiscordID"],
            'prefix' => $row["Prefix"], 'suffix' => $row["Suffix"], 'colour' => $row["Colour"]
        ));
    }

    die(json_encode(array('tdlist' => $data)));
}

http_response_code(204);