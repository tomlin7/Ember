-- export-compile-commands premake module
-- Generates compile_commands.json for clangd/clang-tidy

local p = premake
local project = p.project

p.modules.export_compile_commands = {}
local m = p.modules.export_compile_commands

local workspace = p.workspace

function m.getToolset(cfg)
    return p.tools[cfg.toolset or "msc"]
end

function m.getIncludeDirs(cfg)
    local result = {}
    for _, dir in ipairs(cfg.includedirs) do
        table.insert(result, '-I' .. p.quoted(dir))
    end
    for _, dir in ipairs(cfg.sysincludedirs or {}) do
        table.insert(result, '-isystem ' .. p.quoted(dir))
    end
    return result
end

function m.getDefines(cfg)
    local result = {}
    for _, def in ipairs(cfg.defines) do
        table.insert(result, '-D' .. def)
    end
    return result
end

function m.getCommonFlags(cfg)
    local flags = {}

    if cfg.cppdialect == "C++17" then
        table.insert(flags, "-std=c++17")
    elseif cfg.cppdialect == "C++14" then
        table.insert(flags, "-std=c++14")
    elseif cfg.cppdialect == "C++11" then
        table.insert(flags, "-std=c++11")
    elseif cfg.cppdialect == "C++20" then
        table.insert(flags, "-std=c++20")
    end

    return flags
end

function m.generateCompileCommand(prj, cfg, file)
    local toolset = m.getToolset(cfg)
    local args = {}

    table.insert(args, "clang++")

    -- Add common flags
    for _, flag in ipairs(m.getCommonFlags(cfg)) do
        table.insert(args, flag)
    end

    -- Add defines
    for _, def in ipairs(m.getDefines(cfg)) do
        table.insert(args, def)
    end

    -- Add include dirs
    for _, inc in ipairs(m.getIncludeDirs(cfg)) do
        table.insert(args, inc)
    end

    table.insert(args, "-c")
    table.insert(args, file)

    return {
        directory = path.getabsolute(prj.location or prj.basedir),
        file = path.getabsolute(file),
        command = table.concat(args, " ")
    }
end

function m.onProject(prj)
    local commands = {}

    for cfg in project.eachconfig(prj) do
        -- Only generate for Debug configuration to avoid duplicates
        if cfg.buildcfg == "Debug" then
            local files = prj.files
            for _, file in ipairs(files) do
                if path.iscppfile(file) then
                    local absFile = path.getabsolute(path.join(prj.location or prj.basedir, file))
                    local cmd = m.generateCompileCommand(prj, cfg, absFile)
                    table.insert(commands, cmd)
                end
            end
        end
    end

    return commands
end

function m.execute()
    local commands = {}

    for wks in p.global.eachWorkspace() do
        for prj in workspace.eachproject(wks) do
            local prjCommands = m.onProject(prj)
            for _, cmd in ipairs(prjCommands) do
                table.insert(commands, cmd)
            end
        end
    end

    return commands
end

newaction {
    trigger = "export-compile-commands",
    description = "Generate compile_commands.json for clangd",
    execute = function()
        local commands = m.execute()

        local jsonOutput = "[\n"
        for i, cmd in ipairs(commands) do
            jsonOutput = jsonOutput .. "  {\n"
            jsonOutput = jsonOutput .. string.format('    "directory": "%s",\n', cmd.directory:gsub("\\", "/"))
            jsonOutput = jsonOutput .. string.format('    "file": "%s",\n', cmd.file:gsub("\\", "/"))
            jsonOutput = jsonOutput .. string.format('    "command": "%s"\n', cmd.command:gsub("\\", "/"):gsub('"', '\\"'))
            jsonOutput = jsonOutput .. "  }"
            if i < #commands then
                jsonOutput = jsonOutput .. ","
            end
            jsonOutput = jsonOutput .. "\n"
        end
        jsonOutput = jsonOutput .. "]\n"

        local outFile = io.open("compile_commands.json", "w")
        outFile:write(jsonOutput)
        outFile:close()

        print("Generated compile_commands.json with " .. #commands .. " entries")
    end
}

return m
