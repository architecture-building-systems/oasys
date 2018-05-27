# Import a .csv file and output data streams for easy access to Python simulation results.
#
# Hive: A energy simulation plugin developed by the A/S chair at ETH Zurich
# This component is based on building_physics.py in the RC_BuildingSimulator Github repository
# https://github.com/architecture-building-systems/RC_BuildingSimulator
# Extensive documentation is available on the project wiki.
#
# Author: Justin Zarb <zarbj@student.ethz.ch>
#
# This file is part of Hive
#
# Licensing/Copyright and liability comments go here.
# <Copyright 2018, Architecture and Building Systems - ETH Zurich>
# <Licence: MIT>

"""
Import a .csv file and output data streams for easy 
access to Python simulation results.
-
Provided by Hive 0.0.1
    
    Args:
        path:path of .csv results file
        input_string: Plug in an output of the Zone1 component.
    Returns:
        readMe!: ...
        Zone: A Zone object described by the args.
        input_string: A string which can be copy-pased into a python script running the rc-model
        zone_variables: variables of the Zone object, for diagnostics.
        
"""

ghenv.Component.Name = "Hive_Zone1"
ghenv.Component.NickName = 'ImportPythonResult'
ghenv.Component.Message = 'VER 0.0.1\nMAY_27_2018'
ghenv.Component.IconDisplayMode = ghenv.Component.IconDisplayMode.application
ghenv.Component.Category = "Hive"
ghenv.Component.SubCategory = "4 | Developers"
# ComponentExposure=3


def read_csv(path,col,start,end):
    f = open(path, 'r')
    gains = []
    for line in f.readlines():
        values = line.strip().split(',')
        # do something with values here
        gains.append(values[col])
    return gains[start:end+1]
    f.close()

ff = open(path,'r')
values = ff.readlines()[0].strip().split(',')
ff.close()
for i,j in zip(range(0,len(values)),values):
    print i,':',j

room_depth = input_string[input_string.find('room_depth'):]
room_depth = room_depth[room_depth.find('=')+1:room_depth.find(',')]

room_width = input_string[input_string.find('room_width'):]
room_width = room_width[room_width.find('=')+1:room_width.find(',')]
floor_area = float(room_depth)*float(room_width)

hoy = read_csv(path,0,1,8760)
solar_gains = read_csv(path,10,1,8760)
internal_gains = read_csv(path,7,1,8760)
illuminance = read_csv(path,5,1,8760)
outdoor_air = read_csv(path,9,1,8760)

heating_demand = read_csv(path,3,1,8760)
cooling_demand = read_csv(path,1,1,8760)
indoor_air = read_csv(path,6,1,8760)
heating = heating_demand    

results = 'Python result \n heating demand: %f kWh/m2 \n cooling demand: %f kWh/m2'%(sum([float(h) for h in heating_demand])/(1000*floor_area), sum([float(c) for c in cooling_demand])/(1000*floor_area))
occupancy = [0]*8760