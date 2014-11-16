<?php
define('SECRET_KEY',    'secretword');
define('UPLOAD_DIR',    __DIR__ . '/images');
define('IMAGE_URL',     'http://example.com/images/%s');

$secretKey = (string)filter_input(INPUT_GET, 'secretKey');

if ($secretKey !== SECRET_KEY) {
    http_response_code(403);
    exit;
}

if (false === ($input = @fopen('php://input', 'rb'))) {
    http_response_code(500);
    exit;
}

$baseName = uniqid();
$basePath = substr($baseName, 0, 3);
$fileName = substr($baseName, 3) . '.png';

$dirName = UPLOAD_DIR . '/' . $basePath;

if (!is_dir($dirName)) {
    if (!mkdir($dirName, 0755)) {
        http_response_code(500);
        exit;
    }
}

$filePath = $dirName . '/' . $fileName;

if (false === ($output = @fopen($filePath, 'wb'))) {
    http_response_code(500);
    exit;
}

if (false === stream_copy_to_stream($input, $output)) {
    http_response_code(500);
    exit;
}

fclose($output);

printf(IMAGE_URL, $basePath . '/' . $fileName);
