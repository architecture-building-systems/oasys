# coding=utf-8
"""
Implements predefined default values from SIA 2024:2015 for SIA 380.1 calculations.

"""


def default_values(use_case, area, month):
    # Data according to testcase in sia380.py
    theta_e = [0.5, 1.7, 5.8, 8.9, 14.1, 16.5, 18.5, 18.4, 14.2, 10.4, 4.7, 1.7]
    t = [744.0, 672.0, 744.0, 720.0, 744.0, 720.0, 744.0, 744.0, 720.0, 744.0, 720.0, 744.0]

    # how do you know: A_op, A_w?
    use_case_data = {
        "mfh": {'tau': 182.0,
                'theta_i': 26.0,
                'A_op': 20.0,
                'A_w': 26.0,
                'U_op': 0.2,
                'U_w': 1.2,
                'Vdot_e': 1.0,
                'Vdot_inf': 0.15,
                'eta_rec': 0.75,
                'phi_P': 2.3,
                'phi_B': 2.7,
                'phi_G': 8.0,
                't_P': [434.0, 392.0, 434.0, 420.0, 434.0, 420.0, 434.0, 434.0, 420.0, 434.0, 420.0, 434.0], # 4090.0,
                't_B': [217.0, 196.0, 217.0, 210.0, 217.0, 210.0, 217.0, 217.0, 210.0, 217.0, 210.0, 217.0], # 1540.0,
                't_G': [189.1, 170.8, 189.1, 183.0, 189.1, 183.0, 189.1, 189.1, 183.0, 189.1, 183.0, 189.1], # 1780.0,
                'g': 0.5,
                'f_sh': 84.0,
                'I': [37778.0, 46944.0, 57222.0, 53611.0, 56389.0, 53889.0, 56944.0, 56389.0, 50278.0, 45000.0, 32778.0, 29444.0]},
        "efh": {'tau': 164.0,
                'theta_i': 26.0,
                'A_op': 20.0,
                'A_w': 38.0,
                'U_op': 0.2,
                'U_w': 1.2,
                'Vdot_e': 0.6,
                'Vdot_inf': 0.15,
                'eta_rec': 0.75,
                'phi_P': 1.4,
                'phi_B': 2.7,
                'phi_G': 8.0,
                't_P': [434.0, 392.0, 434.0, 420.0, 434.0, 420.0, 434.0, 434.0, 420.0, 434.0, 420.0, 434.0], # 4090.0,
                't_B': [217.0, 196.0, 217.0, 210.0, 217.0, 210.0, 217.0, 217.0, 210.0, 217.0, 210.0, 217.0], # 1450.0,
                't_G': [189.1, 170.8, 189.1, 183.0, 189.1, 183.0, 189.1, 189.1, 183.0, 189.1, 183.0, 189.1], # 1780.0,
                'g': 0.5,
                'f_sh': 71.0,
                'I': [37778.0, 46944.0, 57222.0, 53611.0, 56389.0, 53889.0, 56944.0, 56389.0, 50278.0, 45000.0, 32778.0, 29444.0]},
        "office": {'tau': 117.0,
                   'theta_i': 26.0,
                   'A_op': 36.0,
                   'A_w': 42.0,
                   'U_op': 0.2,
                   'U_w': 1.2,
                   'Vdot_e': 2.6,
                   'Vdot_inf': 0.15,
                   'eta_rec': 0.75,
                   'phi_P': 5.0,
                   'phi_B': 15.9,
                   'phi_G': 7.0,
                   't_P': [434.0, 392.0, 434.0, 420.0, 434.0, 420.0, 434.0, 434.0, 420.0, 434.0, 420.0, 434.0], # 1500.0,
                   't_B': [217.0, 196.0, 217.0, 210.0, 217.0, 210.0, 217.0, 217.0, 210.0, 217.0, 210.0, 217.0], # 1210.0,
                   't_G': [189.1, 170.8, 189.1, 183.0, 189.1, 183.0, 189.1, 189.1, 183.0, 189.1, 183.0, 189.1], # 1930.0,
                   'g': 0.5,
                   'f_sh': 107.0,
                   'I': [37778.0, 46944.0, 57222.0, 53611.0, 56389.0, 53889.0, 56944.0, 56389.0, 50278.0, 45000.0, 32778.0, 29444.0]},
        "school": {'tau': 72.0,
                   'theta_i': 26.0,
                   'A_op': 70.0,
                   'A_w': 95.0,
                   'U_op': 0.2,
                   'U_w': 1.2,
                   'Vdot_e': 8.3,
                   'Vdot_inf': 0.15,
                   'eta_rec': 0.75,
                   'phi_P': 23.3,
                   'phi_B': 14.0,
                   'phi_G': 4.0,
                   't_P': [434.0, 392.0, 434.0, 420.0, 434.0, 420.0, 434.0, 434.0, 420.0, 434.0, 420.0, 434.0], # 1390.0,
                   't_B': [217.0, 196.0, 217.0, 210.0, 217.0, 210.0, 217.0, 217.0, 210.0, 217.0, 210.0, 217.0], # 1180.0,
                   't_G': [189.1, 170.8, 189.1, 183.0, 189.1, 183.0, 189.1, 189.1, 183.0, 189.1, 183.0, 189.1], # 1770.0,
                   'g': 0.5,
                   'f_sh': 245.0,
                   'I': [37778.0, 46944.0, 57222.0, 53611.0, 56389.0, 53889.0, 56944.0, 56389.0, 50278.0, 45000.0, 32778.0, 29444.0]}
    }

    tmp = use_case_data[use_case]

    return tmp['tau'], theta_e[month - 1], tmp['theta_i'], t[month - 1], tmp['A_op'], tmp['A_w'], tmp['U_op'], \
           tmp['U_w'], tmp['Vdot_e'] * area, tmp['Vdot_inf'] * area, tmp['eta_rec'], tmp['phi_P'] * area, \
           tmp['phi_B'] * area, tmp['phi_G'] * area, tmp['t_P'][month - 1], tmp['t_B'][month - 1], \
           tmp['t_G'][month - 1], tmp['g'], tmp['f_sh'], tmp['I'][month - 1]
