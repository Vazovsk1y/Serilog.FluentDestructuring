param (
    [string]$RowContainingPackageVersion
)

Function GetVersion {
    Param (
        [string]$Row
    )

    if ($Row -match "(?<major>\d+)(\.(?<minor>\d+))?(\.(?<patch>\d+))") {

        $Major = $matches['major']
        $Minor = $matches['minor']
        $Patch = $matches['patch']

        $Version = "$Major.$Minor.$Patch"

        return $Version
    } else {
        return $null
    }
}

GetVersion -Row $RowContainingPackageVersion