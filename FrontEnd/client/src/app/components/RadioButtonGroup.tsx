import { FormControl, RadioGroup, FormControlLabel, Radio } from "@mui/material";

interface Prop{
    options: any[];
    onChange: (event: any) => void;
    selectedValues: string
}

export default function RadioButtonGroup({options, onChange, selectedValues} : Prop){
    return(
        <FormControl>
            <RadioGroup onChange={onChange} value={selectedValues}>
              {options.map(({ value, label }) => (
                <FormControlLabel
                  key={value}
                  value={value}
                  control={<Radio />}
                  label={label}
                />
              ))}
            </RadioGroup>
          </FormControl>
    )
}