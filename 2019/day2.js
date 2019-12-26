const f = function(x) {
  const xs = x.split(",").map(a => parseInt(a));

  for (p = 0; ; p += 4) {
    if (xs[p] != null) {
      //console.log(xs[p]);
      if (xs[p] == 1) {
        //console.log("sum");
        xs[xs[p+3]] = xs[xs[p+1]] + xs[xs[p+2]];
      } else if (xs[p] == 2) {
        xs[xs[p+3]] = xs[xs[p+1]] * xs[xs[p+2]];
      } else if (xs[p] == 99) {
        break;
      } else {
        throw "not expected!";
      }
    } else {
      break;
    }
  }
  return xs.toString();
}

const v = function(x, y, z) {
  const b = x.split(",");
  b[1] = y;
  b[2] = z;
  return b.toString();
}

const g = function(x) {
  const b = x.split(",");
  return parseInt(b[0]);
}

const input = "1,69,79,3,1,1,2,3,1,3,4,3,1,5,0,3,2,1,6,19,1,9,19,23,2,23,10,27,1,27,5,31,1,31,6,35,1,6,35,39,2,39,13,43,1,9,43,47,2,9,47,51,1,51,6,55,2,55,10,59,1,59,5,63,2,10,63,67,2,9,67,71,1,71,5,75,2,10,75,79,1,79,6,83,2,10,83,87,1,5,87,91,2,9,91,95,1,95,5,99,1,99,2,103,1,103,13,0,99,2,14,0,0";
const aim = 19690720;

console.log(f(input));

let a = 0;
for (i = 0; i <= 99; i++) {
  for (j = 0; j <= 99; j++) {
    a = f(v(input, i, j));
    if (g(a) == aim) {
      console.log((i*100)+j);
      break;
    }
  }
  if (g(a) == aim) break;
}



