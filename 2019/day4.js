
var min = 147981;
var max = 691423;
var good = 0;

for (i = 0; i <= 9; i++) {
  for (j = i; j <= 9; j++) {
    for (k = j; k <= 9; k++) {
      for (l = k; l <= 9; l++) {
        for (m = l; m <= 9; m++) {
          for (n = m; n <= 9; n++) {
            var num = (i*100000) + (j*10000) + (k*1000) + (l*100) + (m*10) + n;
			   if (num >= min && num <= max) {
              if (
              (i == j && j != k) ||
              (j == k && i != j && k != l) || 
              (k == l && j != k && l != m) || 
              (l == m && k != l && m != n) ||
              (m == n && l != m)
              ) {
                good++;
              }
            }
          }
        }
      }
    }
  }
}
console.log(good);