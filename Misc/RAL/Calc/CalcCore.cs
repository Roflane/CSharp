namespace Calc;

public static class CalcCore {
    public static double Sum(params double[] numbers) {
        double res = 0;
        foreach (var number in numbers) {
            res += number;
        }

        return res;
    }
    
    public static double Sub(params double[] numbers) {
        double res = numbers[0];
        foreach (var number in numbers) {
            res -= number;
        }

        return res;
    }
    
    public static double Mult(params double[] numbers) {
        double res = numbers[0];
        foreach (var number in numbers) {
            res *= number;
        }
    
        return res;
    }
    
    public static double Div(params double[] numbers) {
        double res = numbers[0];
        foreach (var number in numbers) {
            res /= number;
        }
    
        return res;
    }
}