<?php

$dir          = "./StreamingAssets/Config/3DMotionSimulation";
$return_array = array();

if(is_dir($dir)){

    if($dh = opendir($dir)){
        while(($file = readdir($dh)) != false){

            if($file == "." or $file == ".."){

            } else {
                $return_array[] = $file; // Add the file to the array
            }
        }
    }

    echo json_encode($return_array);
}

?>