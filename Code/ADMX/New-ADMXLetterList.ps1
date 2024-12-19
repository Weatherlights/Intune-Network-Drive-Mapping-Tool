param(
    [string]$FilePath = ".\letter"
)

$letters = @("D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z");

rm "$($FilePath)_admx.xml";
rm "$($FilePath)_string.xml";
rm "$($FilePath)_presentation.xml";


foreach ( $letter in $letters ) {
   $xml = "<policy name=`"Drive_$($letter)`" class=`"User`" displayName=`"`$(string.Drive_$($letter)_DisplayName)`" explainText=`"`$(string.Drive_$($letter)_ExplainText)`" presentation=`"`$(presentation.Drive_$($letter)_Presentation)`" key=`"Software\Policies\weatherlights.com\NetworkDriveMapping\Policies2`" valueName=`"$($letter)`">
      <parentCategory ref=`"Policy`" />
      <supportedOn ref=`"SupportedOn`" />
      <elements>
        <text id=`"Drive_$($letter)_Path`" key=`"Software\Policies\weatherlights.com\NetworkDriveMapping\Policies2\$($letter)`" valueName=`"Path`" required=`"true`" />
        <text id=`"Drive_$($letter)_Username`" key=`"Software\Policies\weatherlights.com\NetworkDriveMapping\Policies2\$($letter)`" valueName=`"Username`" required=`"false`" />
        <text id=`"Drive_$($letter)_Password`" key=`"Software\Policies\weatherlights.com\NetworkDriveMapping\Policies2\$($letter)`" valueName=`"Password`" required=`"false`" />
      </elements>
    </policy>" 

    $string="<string id=`"Drive_$($letter)_DisplayName`">Drive $($letter)</string>
<string id=`"Drive_$($letter)_ExplainText`">Maps network drive $($letter) with the configured path.

Provide a username and password only if authentication should be done with a dedicated user. Do not supply any values if passthrough authentication should be used (recommended).</string>

";

    $presentation="<presentation id=`"Drive_$($letter)_Presentation`">
    <textBox refId=`"Drive_$($letter)_Path`"><label>Path</label><defaultValue></defaultValue></textBox>
    <textBox refId=`"Drive_$($letter)_Username`"><label>Username</label><defaultValue></defaultValue></textBox>
    <textBox refId=`"Drive_$($letter)_Password`"><label>Password</label><defaultValue></defaultValue></textBox>
</presentation>

"


    $xml | Out-File -FilePath "$($FilePath)_admx.xml" -Append
    $string | Out-File -FilePath "$($FilePath)_string.xml" -Append
    $presentation | Out-File -FilePath "$($FilePath)_presentation.xml" -Append
}