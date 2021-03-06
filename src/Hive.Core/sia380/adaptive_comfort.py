# coding=utf-8

from __future__ import division

MONTHS_IN_YEAR = 12
HOURS_IN_YEAR = 8760


def adaptive_comfort(T):
    '''
    Computes adaptive thermal comfort according to Thermal Comfort - PLEA Notes 3. Auliciems and Szokolay 2007
    :param T_m: Ambient temperature in deg C, either averaged monthly or hourly
    :return: Monthly or hourly adaptive thermal comfort temperature T_n, as well as upper and lower bound for 80 and 90 % acceptance
    '''

    # from PLEA NOTES 3 - Thermal Comfort, by Andris Auliciems and Steven V. Szokolay. 2nd edition 2007
    # T_n = 21.5 + 0.11 T_m (Eq. 3.7, for mechanically heated or cooled buildings)
    # where T_n is adaptive thermal comfort temperature and T_m is mean monthly ambient temp
    # for 90% acceptability limits, T_n +/- 2.5 K, for 80 % T_n +/- 3.5 K

    assert len(T) == MONTHS_IN_YEAR or len(T) == HOURS_IN_YEAR, "Only hourly or monthly averaged temperatures are supported."

    setpoints = [0.0] * len(T)
    setpoints_ub_80 = [0.0] * len(T)
    setpoints_lb_80 = [0.0] * len(T)
    setpoints_ub_90 = [0.0] * len(T)
    setpoints_lb_90 = [0.0] * len(T)

    for i, t in enumerate(T):
        sp = 21.5 + 0.11 * t
        setpoints[i] = sp
        setpoints_ub_80[i] = sp + 3.5
        setpoints_lb_80[i] = sp - 3.5
        setpoints_ub_90[i] = sp + 2.5
        setpoints_lb_90[i] = sp - 2.5

    return setpoints, setpoints_ub_80, setpoints_lb_80, setpoints_ub_90, setpoints_lb_90


if __name__ == "__main__":
    T_m = [-4.0, 1.0, 4.4, 6.6, 14.0, 25.0, 18.0, 10.0, 5.0, 1.0, 0.0, -0.4]
    [sp, ub80, lb80, ub90, lb90] = adaptive_comfort(T_m)

    print(sp)
    print(ub80)
    print(lb80)
    print (ub90)
    print (lb90)
