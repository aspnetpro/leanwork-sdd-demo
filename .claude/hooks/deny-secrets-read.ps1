# Bloqueia leitura de arquivos .env / .config (podem conter chaves e configs sensiveis),
# independente da ferramenta ou comando usado (Read, Grep, Bash, PowerShell).
#
# Para Bash/PowerShell exige tambem um verbo de leitura de conteudo (cat, type,
# Get-Content, git show, etc.) alem da referencia ao arquivo, para nao bloquear
# comandos que apenas MENCIONAM ".env"/".config" em texto (ex.: mensagem de commit,
# grep por outra string, git status).
$raw = [Console]::In.ReadToEnd()
try {
    $json = $raw | ConvertFrom-Json
} catch {
    Write-Output '{}'
    exit 0
}

$toolName = $json.tool_name
$filePattern = '(?i)(^|[\\/"'':\s])(\.env(\.[a-zA-Z0-9_-]+)?|[^\\/"'':\s]*\.config)($|[\\/"'':\s])'
$readVerbPattern = '(?i)(\b(cat|type|more|less|head|tail|tac|nl|strings|xxd|od|hexdump|get-content|gc|select-string|sls|findstr|sed|awk|python3?|node|php|ruby|perl|cp|copy|xcopy|robocopy|copy-item)\b|git\s+(show|cat-file|log|diff|blame))'

$blocked = $false

switch ($toolName) {
    'Read' {
        if ($json.tool_input.file_path -match $filePattern) { $blocked = $true }
    }
    'Grep' {
        $target = "$($json.tool_input.path) $($json.tool_input.glob)"
        if ($target -match $filePattern) { $blocked = $true }
    }
    { $_ -in @('Bash', 'PowerShell') } {
        $command = $json.tool_input.command
        if ($command -match $filePattern -and $command -match $readVerbPattern) { $blocked = $true }
    }
    default {
        $blocked = $false
    }
}

if ($blocked) {
    $result = @{
        hookSpecificOutput = @{
            hookEventName            = 'PreToolUse'
            permissionDecision       = 'deny'
            permissionDecisionReason = 'Bloqueado por politica do projeto: leitura de conteudo de arquivos .env/.config (podem conter segredos) nao e permitida, nem via Read/Grep nem via comando de shell (Bash/PowerShell).'
        }
    } | ConvertTo-Json -Depth 5 -Compress
    Write-Output $result
} else {
    Write-Output '{}'
}
