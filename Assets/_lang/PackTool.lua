require("csv_utils")

local cjson = cjson;

--过滤文件
local function filter_file(file_name)
	if string.match(file_name, '%.meta') ~= nil then
		return false
	end
	if string.match(file_name, '%.csv') == nil then
		return false
	end
	return true
end

-- @brief 遍历文件夹下的所有文件
local function over_alls(path, t)
	for entry in os.dir(path) do
		if (entry.type == 'directory') then
			over_alls(path..'/'..entry.name, t);
		else
			table.insert(t, path..'/'..entry.name);
		end
	end
end
local function over_alls_detail(path, t)
	for entry in os.dir(path) do
		if (entry.type == 'directory') then
			over_alls_detail(path..'/'..entry.name, t);
		else
			local dt = {fullname=path..'/'..entry.name, smallname=entry.name, size=entry.size};
			table.insert(t, dt);
		end
	end
end

local function copy_file(ifn, ofn, head)
	local ifile = assert(io.open(ifn, 'rb'));
	local data = ifile:read("*a");
	ifile:close();
	local ofile = assert(io.open(ofn, 'wb'));
	if (head ~= nil) then
		ofile:write(head);
	end
	ofile:write(data);
	ofile:close();
end

--==========================================================

function serialize(obj)
    local lua = ""
    local t = type(obj)
    if t == "number" then
        lua = lua .. obj
    elseif t == "boolean" then
        lua = lua .. tostring(obj)
    elseif t == "string" then
        lua = lua .. string.format("%q", obj)
    elseif t == "table" then
        lua = lua .. "{\n"
        for k, v in pairs(obj) do
			local tn = type(v);
			if (tn ~= "function") and v ~= "" then
				lua = lua .. " [" .. serialize(k) .. "]=" .. serialize(v) .. ",\n"
			end
        end
        local metatable = getmetatable(obj)
        if metatable ~= nil and type(metatable.__index) == "table" then
            for k, v in pairs(metatable.__index) do
				local tn = type(v);
				if (tn ~= "function") then
					lua = lua .. "[" .. serialize(k) .. "]=" .. serialize(v) .. ",\n"
				end
            end
        end
        lua = lua .. "}"
    elseif t == "nil" then
        return nil
    end
    return lua
end

function write_to_lua_file(t, lua_path, file_name)
	local out_name = string.gsub(file_name, ".csv", "")
	local lua_full_path = lua_path..out_name.."_csv.lua";
	
	local ls_t = 'CSV_TABLES = CSV_TABLES or {} \nCSV_TABLES["'..out_name..'"]=';
	local f = assert(io.open(lua_full_path,"w"));
	local ss = serialize(t);
	f:write(ls_t);
	f:write(ss);
	f:close();
	return t;
end

--============================================================

local function build()
	local cur_path = os.currentdir().."/tmp"--"/csv"
	print(cur_path)
	local lua_path = "../Lua/game/configs/";

	local t = {}
	over_alls_detail(cur_path, t)

	for k,v in pairs(t) do
		local file_name = v.smallname
		if filter_file(file_name) then
			print("parse file : " .. v.smallname)
			local csvTable = parse_csv_to_table(v.fullname)-- print(csvTable["11"].Name)
			write_to_lua_file(csvTable, lua_path, v.smallname)
		end
	end

end

build()